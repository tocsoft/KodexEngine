
var net = require('net');
var Q = require("q")

module.exports = function (name, args, callback) {
    
    var deferred = Q.defer();
    
    var pipeName = '\\\\.\\pipe\\kodexclient-' + name || 'default';
    
    var srv = net.createServer(function (socket) {
        socket.on('data', function (str) {
            var args = JSON.parse(str);
            callback(args);
        });
    });
    srv.on('error', function (er) {
        
        //no error we try and conncet to it assuming we errored because another instance has already started
        
        var client = net.connect({ path : pipeName }, function () {
            
            client.end(JSON.stringify(args));
            
            deferred.resolve(false);
        });

    });
    srv.listen(pipeName, function () {
        deferred.resolve(true);
    });
    
    
    
    var p = deferred.promise;
    if (callback) {
        p = p.then(function (v) {
            if (v) {
                callback(args);
            }
        });
    }
    return p;
}