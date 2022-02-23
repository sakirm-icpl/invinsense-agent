import re
import visitor
from app import app
from flask import jsonify,render_template
	
@app.before_request
def do_something_when_a_request_comes_in():
	visitor.track_visitor()

@app.route('/')
def home():
	return jsonify({'msg' : 'hello'})

@app.route('/google')
def google():
	return jsonify({'msg' : 'https://google.com'})

@app.route('/test')
def test():
	return render_template("index.html")
	#return jsonify({'msg' : 'https://google.com'})
		
if __name__ == "__main__":
    app.run(debug=True)