var arguments = process.argv.slice(2);
var calls = 0;

var isSetup = false;
require('./singleinstance.js')('def1', arguments, function () {
    //config if master
    server = require('http').createServer(function (request, response) {
        
        response.end("404: Not Found: " + request.url)
    });

    server.listen(15535);

    var soapServer = require('./soap.js').listen(server, '/api');

    soapServer.on('launch', function (app) {
        //each new launch kicks in here

        //we should show som ui 

        console.log(app);

        setTimeout(function () {

            app.complete({
                set : {
                    "prop1" : "val1"
                },
                run: {
                    "actionname" : "args"
                }
            });
        }, 10000)
    });

}, function (args) {
    //this will be called twice
    calls++;
    console.log(calls)
    console.log(args)
        
});