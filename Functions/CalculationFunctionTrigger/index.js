var restler = require('restler');
var mongoClient = require('mongodb').MongoClient;
var url = 'mongodb://octoparkmongodb:dN7XI06vPDie0H67ZtSnrUocscGNfBsKgKe2bsBK2KNBE5YmRK5cCrtSScWksc0dAB8uWdAVQqq9vcphgJ8rgQ==@octoparkmongodb.documents.azure.com:10250/GlobalStorage?ssl=true';
var Sugar = require('sugar');
var async = require('async');


var COST_PER_MINUTE = 1;

//http://iotmadridaccesscontrolapi.azurewebsites.net/api/GetServiceProfile?accessDeviceId=LK53ABY&accessDeviceType=LicensePlate&locationId=1

var generateOverrun = function(overrun, cb){
    if(overrun.isOverrun){
        mongoClient.connect(url, function (err, db) {
            if(err){
                context.log('[CalculationFunction] err : ', err);
                return cb(err);
            }
            let collection = db.collection('Transactions');

            collection.insertOne(overrun,function(err, result){
                if(err){
                    context.log('Error storing transaction => ', err);
                    
                }
                return cb();
            });
        });
    }
}

module.exports = function(context, req){
    console.log(req);
   // context.log('myQueueItem = ', req);

    let uid = req.params.uid;
    let id = req.params.id;

    context.log(' uid', uid);
    context.log(' => ', id);

    let direction = 0; //myQueueItem.direction;
    let uniqueIdentifier = uid; //myQueueItem.uniqueIdentifier;
    let timeStamp = new Date(); //myQueueItem.timeStamp;

    

    searchCorrespondingRecord(context, 1, uniqueIdentifier,id, timeStamp, function(err, result){
        context.log('1.....', result);
        searchCurrentRecord(context, 1, id, function(err, current){
            context.log('Do Calculation on : ');
            context.log(' in => ', result);
            context.log(' out => ', current);
            if(current == null){
                return context.done();
            }
            getServiceProfile(uid, 1, function(serviceProfile){
                context.log('Service Profile : ', serviceProfile);
                getOverrunsPerDay(context, result, current, serviceProfile, uniqueIdentifier, function(err, overruns){
                    async.each(overruns, generateOverrun, function(){
                        context.log('Finished generating transactions');
                        return context.done();             
                    })
                    
                })
            })
        });
        if(err){
            ///do something something... 
        }

       
    });
    
}//test

var getDays = function(startDate, stopDate){
    var dateArray = new Array();
    var currentDate = startDate;
    while (currentDate <= stopDate) {
        dateArray.push( new Date (currentDate) )
        currentDate = Sugar.Date.addDays(currentDate,1);
    }
    return dateArray;
}

var getOverrunsPerDay = function(context, driveIn , driveOut, sp, uid, cb){
    let days = getDays(new Date(driveIn.timeStamp), new Date(driveOut.timeStamp));
    context.log('Check days : ', days);
    let overruns = new Array(days.length);
    days.forEach(function(item, index){
        var dayOfWeek = Sugar.Date.getWeekday(new Date(item));
        context.log('check day: ', item , dayOfWeek);
        var foundServiceProfile = Sugar.Array.find(sp, function(s){
            return s.DayOfWeek == dayOfWeek;
        });
        var d = new Date();
        let startS = new Date(d.getFullYear(),d.getMonth(),d.getDate(),foundServiceProfile.StartHour,foundServiceProfile.StartMinutes,0);
        let endS = new Date(d.getFullYear(),d.getMonth(),d.getDate(),foundServiceProfile.EndHour,foundServiceProfile.EndMinutes,0);
        
        console.log('startS : ', startS, driveIn.timeStamp);
        console.log('endS : ', endS, driveIn.timeStamp);

        let isAfterStart = Sugar.Date.isAfter(new Date(driveIn.timeStamp), startS);
        let isBeforeEnd = Sugar.Date.isBefore(new Date(driveIn.timeStamp), endS);

        let endTime = new Date(driveOut.timestamp);

        let exitBeforeEnd = Sugar.Date.isBefore(endTime, endS);

        let isOverrun = false;
        let overTimeInMinutes = 0;

        if(isAfterStart && !exitBeforeEnd){
            isOverrun = true;
            overTimeInMinutes = Sugar.Date.range(endS, endTime).minutes();
        }
        context.log('Checking : ', isAfterStart, isBeforeEnd);

        overruns[index] = {
            isOverrun : !(isAfterStart && isBeforeEnd),
            timeInOverrun : overTimeInMinutes,
            costs : overTimeInMinutes * COST_PER_MINUTE,
            uid : uid
        }

    });

    return cb(overruns);
}

var getServiceProfile = function(uid, locationId, cb){
    var targetUrl = 'http://iotmadridaccesscontrolapi.azurewebsites.net/api/GetServiceProfile?accessDeviceId=' + uid + '&accessDeviceType=LicensePlate&locationId=' + locationId;

    restler.get(targetUrl).on('complete', function(result){
        return cb( result);
    });
}

var searchCurrentRecord = function(context, direction, id, cb){
    mongoClient.connect(url, function (err, db) {
        if(err){
            context.log('[CalculationFunction] err : ', err);
            return cb(err);
        }
        let collection = db.collection('AccessLog');
        var searchObject = {
            //direction : 0, 
            //uniqueIdentifier : uid,
            trackid : id
        };
        var found = false;
        var foundItem;
        context.log('start search current recor ... ', searchObject);
        collection.findOne(searchObject, function(err, item){
            if(err){
                context.log('err : ', err);
            }
            context.log('item : ', item);
            if(typeof(item) != "undefined"){
                found = true;
                foundItem = item;
            }
            return cb(null, foundItem);
        });
         
        
    })
   
}

var searchCorrespondingRecord = function(context, direction, uid, id, timestamp, cb){
    context.log('[CalculationFunction] url : ', url);
    mongoClient.connect(url, function (err, db) {
        if(err){
            context.log('[CalculationFunction] err : ', err);
            return cb(err);
        }
        let collection = db.collection('AccessLog');
        var searchObject = {
            direction : 0, 
            uniqueIdentifier : uid,
            //trackid : id
        };

        context.log(' <===== ' , searchObject);
        collection.find(searchObject).sort({timeStamp : -1}).toArray(function (err, items){
            if(err){
                context.log('[CalculationFunction] error retrievign data ', err);
                db.close();
                return context.done();
            }
            //context.log('[CalculationFunction] Found : ' ,items);
            db.close();
            if(items.length > 0){
                return cb(err, items[0]);
            }
            return cb(err, null);            
        });

    })
}

/*
console.done = function(){}
v(console,{
    params : {
        uid : 'LK53ABY',
        id : 'cj0nleac70001f8o0mg5h7ubb'
    }

})*/