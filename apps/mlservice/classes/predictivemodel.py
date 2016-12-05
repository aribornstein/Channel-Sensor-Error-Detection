import json
import os
import pickle
from sys import argv
import numpy as np 
from . import protomodel


class predictivemodel:
    def __init__(self):
        file_path = os.path.relpath("models/{0}".format("test"))
        self.loadmodel(file_path)
        clf =''

    def trainmodel(self, csvid):
        model = protomodel.protomodel()
        serializedmodel = model.train(csvid,'regression')
        file_path = os.path.relpath("models/test")
        print(file_path)
        target = open(file_path, 'w') 
        target.writelines(serializedmodel)
        target.close()

    def loadmodel(self,filename):
        if os.path.exists(filename):
            f = open(filename, 'r')
            model = f.read()
            self.clf = pickle.loads(model)
    
    def getprediction(self, datavalue):
        b_new = json.loads(datavalue)
        a_new = np.array(b_new)
        print("blabnidfanfidosaidsjdjfdisajfoisa")
        print('{}{}'.format("tester ",a_new[3]))
        return self.clf.predict(a_new)
