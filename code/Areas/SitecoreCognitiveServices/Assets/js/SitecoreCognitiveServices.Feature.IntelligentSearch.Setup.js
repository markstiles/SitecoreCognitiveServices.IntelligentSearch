jQuery.noConflict();

//setup
jQuery(document).ready(function () {
    //handles setup form
    var setupForm = ".setup-form";
    jQuery(setupForm + " button")
        .click(function (event) {
            event.preventDefault();

            var overwriteValue = jQuery(setupForm + " input[name='overwriteOption']:checked").val();
            var luisValue = jQuery(setupForm + " #luisApi").val();
            var luisEndpointValue = jQuery(setupForm + " #luisApiEndpoint").val();
            var textAnalyticsValue = jQuery(setupForm + " #textAnalyticsApi").val();
            var textAnalyticsEndpointValue = jQuery(setupForm + " #textAnalyticsApiEndpoint").val();
            
            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(setupForm).attr("action"),
                {
                    overwriteOption: overwriteValue,
                    luisApi: luisValue,
                    luisApiEndpoint: luisEndpointValue,
                    textAnalyticsApi: textAnalyticsValue,
                    textAnalyticsApiEndpoint: textAnalyticsEndpointValue
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(".result-failure .item-list").text(r.Items);
                    jQuery(".result-failure").show();
                } else {
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
            });
        });
});