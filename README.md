# yggdrasil

Simple online backbone for small games.

Can be auto deploy on heroku easily. Necessitate mongoDB database (free atlas instance)

Heroku environment variable required:
- MONGODB_URI
- MONGODB_DB_NAME
- MONGODB_COLLECTION_NAME_PLAYERRECORD
- MONGODB_COLLECTION_NAME_ITEMSTORE
- JWT_KEY

Support:
  - account creation
  - login with jwt tokens
  - item store
  - player inventory
  - item unlocking (admin calls)
  - player notification (player => player and admin => player, necessitate polling)
