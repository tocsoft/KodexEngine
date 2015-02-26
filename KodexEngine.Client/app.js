var arguments = process.argv.slice(2);
var calls = 0;

var isSetup = false;
require('./singleinstance.js')('def', arguments, function (args) {
    //this will be called twice
    calls++;
    console.log(calls)
    console.log(args)
    
    if (!isSetup) {
        isSetup = true;
        var server = require('./soap.js')(15535);
        server.on('launch', function (app) { 
            //each new launch kicks in here

            //we should show som ui 

            console.log(app);
        });
    }
});