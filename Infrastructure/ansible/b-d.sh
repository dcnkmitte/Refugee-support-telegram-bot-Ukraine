ansible-playbook --inventory-file inventory \
--extra-vars "telegram_access_token=$TELEGRAM_ACCESSTOKEN directus_access_token=$DIRECTUS_ACCESSTOKEN deployment_version=$DEPLOYMENT_VERSION" \
bot-deployment.yml \
-kK