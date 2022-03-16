ansible-playbook --inventory-file inventory \
--extra-vars "directus_cms_key=$DIRECTUS_CMS_KEY directus_cms_secret=$DIRECTUS_CMS_SECRET directus_cms_admin_email=$DIRECTUS_CMS_ADMIN_EMAIL directus_cms_admin_password=$DIRECTUS_CMS_ADMIN_PASSWORD" \
directus-deployment.yml \
-kK
