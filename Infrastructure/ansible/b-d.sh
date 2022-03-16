ansible-playbook --inventory-file inventory \
--extra-vars "telegram_access_token=$TELEGRAM_ACCESSTOKEN directus_password=$DIRECTUS_PASSWORD directus_email=$DIRECTUS_EMAIL deployment_version=$DEPLOYMENT_VERSION" \
bot-deployment.yml \
-kK
