var soap = require('soap-server');

function Application(app, user, caseref, args) {
    this.app = app;
    this.user = user;
    this.caseref = caseref;
    this.args = args;

    this.busy = true;
    this.result = null;
}

Application.prototype.complete = function (result) {
    this.busy = false;
    this.result = result;        
}

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

function MyTestService(emitter) {
    this.emitter = emitter;
}

MyTestService.prototype.launch = function (app, user, caseref, args) {

    this.emitter.emit("launch", getAppOrCreate(app, user, caseref, args));
    return true;

};

MyTestService.prototype.isbusy = function (app, user) {
    var app = getApp(app, user);
    
    if (app) {
        return app.busy;
    }
    return false;
};

MyTestService.prototype.result = function (app, user) {
    var app = getApp(app, user);
    
    if (app && app.busy) {
        return app.result;
    }
    return "";
};




module.exports = function (port) {
    var emitter = new require('events').EventEmitter();
    
    var soapServer = new soap.SoapServer();
    var soapService = soapServer.addService('api', new MyTestService(emitter));
        
    soapServer.listen(port, 'localhost', function () {
        var t = arguments;
    });
    
    return emitter;
}