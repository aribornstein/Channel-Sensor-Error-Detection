import datetime 
import numpy as np 
import pandas as pd 
import matplotlib.pyplot as plt 
from sklearn import cross_validation 
from sklearn.linear_model import LogisticRegression 
from sklearn.ensemble import RandomForestClassifier 
from sklearn.metrics import classification_report 
from sklearn.metrics import confusion_matrix 
from pylab import rcParams 
import pickle
import json
import os

class protomodel:
    def train(self, csvpath, mode):
        """
        trains the selected predictive model and returns printed report
        selects algorithm based on mode variable mode = ['simple', 'decisiontrees','regression']
        returns serialized model
        """
        print("test")
        df = pd.read_csv(csvpath).set_index('time')
        df['diff'] = np.abs(df.velocity - df.velocity_edited) 
        df['anomaly'] = df['diff'].apply(lambda x: 1 if x > 0 else 0) 
        columns = ['level', 'velocity', 'level_1', 'velocity_1', 'level_2', 'velocity_2', 'level_3', 'velocity_3', ] 
        X_train, X_test, y_train, y_test = cross_validation.train_test_split(df[columns], df.anomaly, test_size=0.3, random_state=0)
        
        file_pathx = os.path.relpath("models/xtest")
        file_pathy = os.path.relpath("models/ytest")
        # print(json.dumps(X_test[1:5].tolist(), cls=SetEncoder))
        # print(json.dumps(y_test[1:5].tolist(), cls=SetEncoder))
        
        ytest = json.dumps(y_test.tolist(), cls=SetEncoder)
        xtest = json.dumps(X_test.tolist(), cls=SetEncoder)

        target = open(file_pathx, 'w') 
        target.writelines(xtest)
        target.close()
  
        target = open(file_pathy, 'w') 
        target.writelines(ytest)
        target.close()
  
        xs = pd.core.series.Series()
        clf = LogisticRegression()
        
        if mode == 'simple':
            xs = X_test['velocity'].apply(lambda x: 1 if x == 0 else 0)
            print(type(xs))
        elif mode == 'regression':
            clf = LogisticRegression()            
            clf.fit(X_train, y_train)
            xs = clf.predict(X_test) 
            print(json.dumps(X_test[1:5].tolist(), cls=SetEncoder))
            print(json.dumps(y_test[1:5].tolist(), cls=SetEncoder))
        
            # print(type(clf))
        elif mode == 'decisiontrees':
            clf = RandomForestClassifier()
            clf.fit(X_train, y_train)
            xs = clf.predict(X_test) 
        self.print_report(y_test, xs)

        return pickle.dumps(clf)

    def __init__(self):
        test=''

    def print_report(self, expected, predicted):
        target_names = ['Anomalies', 'Regular velocity'] 
        print("Confusion Matrix") 
        print(confusion_matrix(expected, predicted))
        print(classification_report(expected, predicted, target_names = target_names))


class SetEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, set):
            return list(obj)
        return json.JSONEncoder.default(self, obj)
