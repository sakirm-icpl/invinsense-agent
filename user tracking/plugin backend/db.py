from distutils.command.config import config
from app import app
#from flaskext.mysql import MySQL
import mariadb


def createConnection():
    conn = mariadb.connect(
         host='localhost',
         port= 3306,
         user='root',
         password='root',
         database='invinsense')
    print(conn)
    return conn
#End Function

# MySQL configurations
# app.config['mariadb_DATABASE_USER'] = 'root'
# app.config['mariadb_DATABASE_PASSWORD'] = 'root'
# app.config['mariadb_DATABASE_PORT'] = '3306'
# app.config['mariadb_DATABASE_DB'] = 'invinsense'
# app.config['mariadb_DATABASE_HOST'] = 'localhost'
#mariadb.init_app(app)