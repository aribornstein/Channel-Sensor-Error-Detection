#!/usr/bin/env python
import xml.etree.ElementTree as ET
import web
import json 
from classes import predictivemodel


model = predictivemodel.predictivemodel()

urls = (
    '/train', 'train_model',
    '/predict', 'predict'
)

app = web.application(urls, globals())

class train_model:        
    def GET(self):
        return str(model.trainmodel('./data/dataset1.csv'))

class predict:
    def POST(self):
        data = json.loads(web.data())
        print(data)
        return model.getprediction(web.data())[0]

if __name__ == "__main__":
    app.run()
   
