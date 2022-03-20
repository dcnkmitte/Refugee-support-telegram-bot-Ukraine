ansible-playbook --inventory-file inventory \
--extra-vars "seq_admin_password=$SEQ_ADMIN_PASSWORD" \
infra-deployment.yaml \
-kK
