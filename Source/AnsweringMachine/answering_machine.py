
from sklearn.metrics.pairwise import cosine_similarity
from os.path import exists
import os
import pickle
import time
import requests


DIRECTUS_TOKEN = os.getenv('DIRECTUS_TOKEN')
THRESHOLD = os.getenv('THRESHOLD')


def get_answers(similarAnswers):

    idList = [s[0][1][0] for s in similarAnswers]
    ids = ','.join(map(str, idList))

    main_url = 'http://host.docker.internal:8055/items/fragen?access_token=' + DIRECTUS_TOKEN
    url = main_url + '&filter[id][_in]=' + ids + '&filter[antwort][_nnull]=true'

    resp = requests.get(url)
    data = resp.json()["data"]

    resultList = enrich_results_with_answers_origins(idList, main_url, data)

    return resultList

def enrich_results_with_answers_origins(idList, main_url, data):
    
    manualy_answered_backend_questions = set([s["beantwortetmit"] for s in data if s["beantwortetmit"]
                      != None if s["beantwortetmit"] not in idList])
    if len(manualy_answered_backend_questions) > 0:
        ids = ','.join(map(str, manualy_answered_backend_questions))
        url = main_url + '&filter[id][_in]=' + ids + \
            '&filter[antwort][_nnull]=true&filter[beantwortetmit][_null]=true'
        resp = requests.get(url)
        orginalData = resp.json()["data"]
        resultList = [x for x in data +
                      orginalData if x["beantwortetmit"] is None]
    else:
        resultList = [x for x in data if x["beantwortetmit"] is None]
    return resultList


def sendAnswersBack(answers, inputQuestionJson):
    """Post new answer to the imputQuestion object
    create the Answer from similar answers from past"""

    url = 'http://host.docker.internal:8055/items/fragen/' + \
        str(inputQuestionJson["key"])+'?access_token=dasisttoken'

    patchData = '\n\n'.join(
        ['<b>'+x["frage"] + '</b>\n<i>' + x["antwort"]+'</i>' for x in answers])

    resp = requests.patch(
        url, json={"antwort": patchData, "beantwortetmit": answers[0]["id"]})

    return resp


def process_answering_question(inputObject, model):

    threshold = float(THRESHOLD)
    maxAttempts = 30
    attempt = 0

    while True:
        try:
            if exists("sentences"):
                with open("sentences", "rb") as fp:
                    sentencesObject = pickle.load(fp)
                    sentencesObject = process_auto_answering(
                        inputObject, threshold, model, sentencesObject)
            else:
                sentencesObject = process_auto_answering(
                    inputObject, threshold, model)
            with open("sentences", "w+b") as fp:
                pickle.dump(sentencesObject, fp)
            break
        except:
            attempt += 1
            if attempt == maxAttempts:
                break
            time.sleep(1)


def process_auto_answering(inputQuestionJson, threshold, model, sentencesObject=None):

    question = inputQuestionJson["payload"]["frage"]
    id = inputQuestionJson["key"]
    sentence = model.encode(question)
    if sentencesObject == None:
        return [(id, question, sentence)]

    encodedQuestion = [s[2] for s in sentencesObject]
    cosine = cosine_similarity([sentence], encodedQuestion)

    similar_questions = [(a, b) for a, b in zip(
        enumerate(sentencesObject), cosine[0]) if b > threshold]

    if len(similar_questions) > 0:
        similar_questions.sort(key=lambda tup: tup[1], reverse=True)
        similar_questions_answers = get_answers(similar_questions)
        if len(similar_questions_answers) > 0:
            sendAnswersBack(similar_questions_answers, inputQuestionJson)

    sentencesObject.append((id, question, sentence))
    return sentencesObject
