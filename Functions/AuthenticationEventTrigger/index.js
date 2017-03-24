var restler = require('restler');
var Client = require('azure-iothub').Client;
var Message = require('azure-iot-common').Message;
var cuid = require('cuid');

//               http://iotmadridaccesscontrolapi.azurewebsites.net/api/Access?accessDeviceId=LK53ABY         &accessDeviceType=LicensePlate&locationId=1
var targeturl = 'http://iotmadridaccesscontrolapi.azurewebsites.net/api/Access?accessDeviceId={accessDeviceId}&accessDeviceType={accessDeviceType}&locationId={locationid}';


var directionEnum = {
    in : 0,
    out : 1
}

module.exports = function(context, myQueueItem) {
    context.log('<== start ==>');
	let targetDeviceId = myQueueItem.accessDeviceValue;
	let topic = myQueueItem.type;
    let targetAzureDeviceId = myQueueItem.device;
    let correlationId = myQueueItem.correlation_id ? myQueueItem.correlation_id : '';
	let accessDeviceType = myQueueItem.accessDeviceType
    let locationId = myQueueItem.locationId;
    let direction = myQueueItem.direction;
    let timeStamp = myQueueItem.timestamp;


	let connectionString = process.env.octopark_iot_hub ? process.env.octopark_iot_hub : 'ERROR';
	//gonna use AUTH/REQ with an authorization request
	let messageObjectToReturn = {
		messageType: 'AUTH/FAIL',
        correlation_id : correlationId
	}
    context.log('===> ', targetDeviceId, topic);
	if (targetDeviceId && topic == 'AUTH/REQUEST') {
		var client = Client.fromConnectionString(connectionString);
        targeturl = targeturl.replace('{accessDeviceId}',targetDeviceId);
        targeturl = targeturl.replace('{accessDeviceType}', 'LicensePlate'); //TODO : Nicky => replace hard - coded value with value from myQueueItem
        targeturl = targeturl.replace('{locationid}', locationId);
        context.log('== ', targeturl);
        restler.get(targeturl).on('complete', function(result){
            context.log('[AuthenticationFunction] Response from API : ', result);
            if(result && typeof(result) != "undefined"){
                messageObjectToReturn.messageType = 'AUTH/SUCCESS';
            }
            
            client.open(function(err) {
                if (err) {
                    return context.done();
                }
                var data = JSON.stringify(messageObjectToReturn);
                var message = new Message(data);
                client.send(targetAzureDeviceId, message, function(err, res) {
                    context.log('[AuthenticationFunction] send data to documentdb');
                    let documentToStore = {
                        trackid : cuid(),
                        direction : direction == 'in' ? directionEnum.in : directionEnum.out,
                        uniqueIdentifier : targetDeviceId,
                        timeStamp : timeStamp,///
                        correlationId : correlationId
                    }
                    context.log('[OutputDocument] Sending to store : ', documentToStore);
                    context.bindings.accessLogDocument = documentToStore,
                    context.bindings.outputDocument = myQueueItem;
                    // https://{function app name}.azurewebsites.net/api/{function name}
                    
                    if(direction == 'out'){
                        context.log('Trigger Calculation');
                        let code = 'jtKMMrlUNSLXqoQ6YoPwILQt3Px9pffk5c0yhNSUi6YpxwPGUgk88Q==';
                        let targetTriggerUrl = ' https://testfunctionappnicky.azurewebsites.net/api/calculate/' + targetDeviceId+ '/' + documentToStore.trackid  + '/?code=' + code;
                        context.log('Reqeust url : ', targetTriggerUrl);
                    
                        restler.get(targetTriggerUrl).on('complete', function(result){
                            context.log('[AuthenticationFunction] done setting output bindings');
                            context.done();
                        });//test
                    }else{
                        context.done();
                    }
                });

            });
        });
        
	}else{
        context.done();
    }
	
};
