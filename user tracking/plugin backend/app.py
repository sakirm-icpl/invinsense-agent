from flask import Flask
from datetime import timedelta
	
app = Flask(__name__)
app.secret_key = "abc"
app.config['PERMANENT_SESSION_LIFETIME'] =  timedelta(minutes=30)