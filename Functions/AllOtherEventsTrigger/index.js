module.exports = function (context, myEventHubTrigger) {
    context.log('JavaScript eventhub trigger function processed work item:', myEventHubTrigger);
    context.bindings.outputDocument = myEventHubTrigger;
    context.done();
};