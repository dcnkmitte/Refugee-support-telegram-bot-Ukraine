from answering_machine import process_answering_question
from flask import Flask, request
from sentence_transformers import SentenceTransformer
import logging
import sys


sys.path.append("/")


app = Flask(__name__)

model = SentenceTransformer("sentence-transformers/distiluse-base-multilingual-cased-v1")

# GET requests will be blocked
@app.route('/json-example', methods=['POST'])
def auto_answering():
    request_data = request.get_json()
    app.logger.addHandler(logging.StreamHandler())
    app.logger.setLevel(logging.INFO)
    app.logger.info(request_data)
    process_answering_question(request_data, model)
    return request_data

