var loadedApps = [];

function Application(app, user, caseref, args) {
    this.app = app;
    this.user = user;
    this.caseref = caseref;
    this.args = args;
    this.cancelled = false;
    
    this.id = (new Date()).getTime();
    
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
    getApp : function (appNameOrId, user) {
        
        for (var i = 0; i < loadedApps.length; i++) {
            var app = loadedApps[i];
            
            if (user) {
                if (app.user == user && app.app == appNameOrId) {
                    return app;
                }
            } else {
                if (app.id == appNameOrId) {
                    return app;
                }
            }
        }
        return null;
    },
    getAppOrCreate : function (app, user, caseref, args) {
        
        var current = getApp(app, user);
        if (current) {
            return cuurent;
        }
        
        var newApp = new Application(app, user, caseref, args)
        
        loadedApps.push(newApp);
        return newApp;
    }
};