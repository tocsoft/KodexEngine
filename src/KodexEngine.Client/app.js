var arguments = process.argv.slice(2);
var calls = 0;

var config = require('./config.json');

var apps = require('./active-apps.js');

var isSetup = false;
require('./singleinstance.js')('def1', arguments, function () {
    
    var app = require('express')()
    
    app.post('/result', function (req, res) {
        
        var data = '';
        req.setEncoding('utf8');
        req.on('data', function (chunk) {
            data += chunk;
        });
        
        req.on('end', function () {
            
            var content = JSON.parse(data)
           
            var current = apps.getApp(content.sessionId);
            
            current[content.action](current.result);

            res.send("OK");
        });
    });
    
    //config if master
    server = require('http').createServer(app);
    
    var soapServer = require('./soap.js').listen(server, '/api');
    
    server.listen(15535);
    
    soapServer.on('launch', function (app) {
        
        //each new launch kicks in here
        
        var spawn = require('child_process').spawn;
        var ps = spawn('.\\node_modules\\nw\\nwjs\\nw.exe', ['Browser', 15535, config.server, app.app, app.id, app.args, app.user]);
       
    });

}, function (args) {
    //this will be called twice
    calls++;
    console.log(calls)
    console.log(args)
        
},
function (v) {
    debugger;
    process.exit(1);
    
});


