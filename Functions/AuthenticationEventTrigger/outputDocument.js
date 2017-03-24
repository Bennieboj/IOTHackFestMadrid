var OutputDocument = function(){}

var directionEnum = {
    in : 0,
    out : 1
}

/**
 * Save an accessEvent ... and store it in DocumentDB
 * This db is being used to save in and out events for accessdeviceIds
 */
OutputDocument.prototype.saveAccessEvent = function(context, outputBindingDocument, direction, uniqueIdentifier, timeStamp, correlationId){
    let documentToStore = {
        direction : direction == 'in' ? directionEnum.in : directionEnum.out,
        uniqueIdentifier : uniqueIdentifier,
        timeStamp : timeStamp,
        correlationId : correlationId
    }
    context.log('[OutputDocument] Sending to store : ', documentToStore);
    outputBindingDocument = documentToStore;
}

module.exports = new OutputDocument();