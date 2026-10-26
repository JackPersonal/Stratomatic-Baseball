from flask import Flask, render_template

app = Flask(__name__)


@app.route("/") #default
def index():
    return render_template("index.html"), 200

@app.route("/test")
def test():
    return render_template("index.html"), 200

@app.route("/gameplay")
def gameplay():
    return render_template("baseball_diamond_ui.html"), 200

@app.route("/boxscore")
def box_score():
    return render_template("box_score_ui.html"), 200

app.run("0.0.0.0", port=7808, debug = True) 