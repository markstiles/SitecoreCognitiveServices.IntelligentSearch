jQuery.noConflict();

//query application
jQuery(document).ready(function () {
    //query
    var queryForm = ".query-form";
    var queryError = ".query-error";
    var appId = queryForm + " #app-id";
    var queryInput = queryForm + " #query-value";
    var formSubmit = queryForm + " .form-submit";
    var progressIndicator = ".progress-indicator";
    var queryResultValue = ".query-result-value";

    
    jQuery(queryInput).keydown(function (e) {
        if (e.which !== 13)
            return;

        e.preventDefault();
        jQuery(formSubmit).click();
    });

    jQuery(formSubmit).click(function(event) {
        event.preventDefault();

        var queryValue = jQuery(queryInput).val();
        if (queryValue === "")
            return;

        jQuery(queryInput).val("");
        
        GetQueryResult(queryValue);
    });

    function GetQueryResult(queryValue)
    {
        var appIdValue = jQuery(appId).val();
       
        jQuery(progressIndicator).show();

        jQuery.post(
            jQuery(queryForm).attr("action"),
            {
                applicationId: appIdValue,
                query: queryValue
            }
        ).done(function (r) {
            jQuery(queryResultValue).val(JSON.stringify(r, null, 2));
            jQuery(progressIndicator).hide();
        });
    }

    jQuery(queryInput).focus();

    var appIdValue = jQuery(appId).val();
    if (appIdValue === "")
    {
        jQuery(queryForm).hide();
        jQuery(queryError).show();
    }
});