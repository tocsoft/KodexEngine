var soap = require('soap');
var apps = require('./active-apps.js');

module.exports = {
    
    listen: function (server, path) {
        
       
        var events = require('events');
        var eventEmitter = new events.EventEmitter();
        
        
        var myService = {
            api: {
                apiSoap: {
                    launch: function (args) {
                        if (apps.getApp(args.app, args.user)) {
                            return { launchResult: false };
                        }
                        var app = apps.getAppOrCreate(args.app, args.user, args.caseref, args.args);
                        eventEmitter.emit("launch", app);
                        return { launchResult: !app.cancelled };
                        
                    },
                    
                    // This is how to define an asynchronous function.
                    isbusy: function (args, callback) {
                        
                        var app = apps.getApp(args.app, args.user);
                        
                        if (app) {
                            var timeoutcount = 0;
                            var check;
                            check = function () {
                                timeoutcount++;
                                if (app.busy && timeoutcount < 10) {
                                    setTimeout(function () {
                                        check();
                                    }, 500);
                                } else {
                                    callback({ isbusyResult: app.busy });
                                }
                            }
                            
                            check();
                        } else {
                            callback({ isbusyResult: false });
                        }

                    },
                    
                    // This is how to receive incoming headers
                    results: function (args, cb, headers) {
                        var app = apps.getApp(args.app, args.user);
                        
                        if (app && !app.busy) {
                            //serialize the result into a format proclaim will be able to action generically
                            
                            //lets just stringify for now
                            var res = JSON.stringify(app.result);
                            
                            return { resultsResult: res };
                        }
                        return { resultsResult: "" };
                    }
                }
            }
        }
        
        var xml = require('fs').readFileSync('api.wsdl', 'utf8');
        
        
        
        //server.addListener('request', function (request, response) {
        //    if (request.url == path+'?wsdl') {
        //        response.end(xml);
        //    }
        //});
        
        soap.listen(server, path, myService, xml);
        
        
        return eventEmitter;
    }
}