let html = """
    <html>
        <head>
            <title>Suave TODO</title>
            <link href="/static/style.css" type="text/css" rel="stylesheet">
            <link href='//fonts.googleapis.com/css?family=Source+Sans+Pro:400,700' rel='stylesheet' type='text/css'>
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>
        <body ng-app="Todo" ng-cloak>    
            <div ng-controller="TodoList as todo" id="container">
                
                <h1>Todo</h1>
                <ul>
                    <li><input type="text" ng-model="todo.text" placeholder="New todo item.."> <a href="#" ng-click="todo.create()">[ADD]</a></li>
                    <li ng-repeat="item in todo.todos"><a href="#" ng-click="todo.remove(item.id)">[DONE]</a> {{ item.text }}</li>
                </ul>
                                
            </div>
            <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.3.15/angular.min.js"></script>
            <script src="/static/app.js"></script>
        </body>
    </html>
"""

let script = """
    (function() {
        
        var app = angular.module("Todo", []);
        
        app.controller("TodoList", function($scope, $http) {    
            var self = this;
            
            self.text = "";
            self.todos = [];
           
            var load = function() { 
                $http.get('/todos').success(function(data) {
                    self.todos = data.todos;
                }); 
            };

            self.create = function () {
                var text = self.text; 
                $http.post('/todos', "text=" + encodeURIComponent(text)).then(function() {
                    self.text = "";
                    load();
                });
            };
            
            self.remove = function (id) {
                $http.delete('/todos/' + id).then(function() {
                    load();
                });
            };
            
            load();
        });
    })();
"""

let style = """
    html, body { margin: 0; padding: 0; width: 100%; height: 100%; }
    #container { margin: 50px; font-family: "Source Sans Pro", sans-serif; font-weight: 400; }
        h1 { font-weight: 700; }
        a { text-decoration: none; }
        li { margin: 20px 0 0 0; }
        input { padding: 10px; }
"""