var soap = require('soap');

function Application(app, user, caseref, args) {
    this.app = app;
    this.user = user;
    this.caseref = caseref;
    this.args = args;
    this.cancelled = false;

    this.busy = true;
    this.result = null;
}

Application.prototype.complete = function (result) {
    this.busy = false;
    this.result = result;
}

Application.prototype.cancel = function () {
    this.cancelled = true;
}


module.exports = {

    listen: function (server, path) {

    var loadedApps = {};

    function getAppOrCreate(app, user, caseref, args) {

        loadedApps[user] = loadedApps[user] || {};

        loadedApps[user][app] = loadedApps[user][app] || new Application(app, user, caseref, args);

        return loadedApps[user][app];
    }

    function getApp(app, user) {

        loadedApps[user] = loadedApps[user] || {};

        if (loadedApps[user][app]) {
            return loadedApps[user][app];
        }
        return null;
    }
    var events = require('events');
    var eventEmitter = new events.EventEmitter();

    
    var myService = {
        api: {
            apiSoap: {
                launch: function (args) {

                    if (getApp(args.app, args.user))
                    {
                        return { launchResult: false };
                    }
                    var app = getAppOrCreate(args.app, args.user, args.caseref, args.args);
                    eventEmitter.emit("launch", app);
                    return { launchResult: !app.cancelled };
                },

                // This is how to define an asynchronous function.
                isbusy: function (args, callback) {

                    var app = getApp(args.app, args.user);

                    if (app) {
                        var timeoutcount = 0;
                        var check;
                        check = function() {
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
                    var app = getApp(args.app, args.user);

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