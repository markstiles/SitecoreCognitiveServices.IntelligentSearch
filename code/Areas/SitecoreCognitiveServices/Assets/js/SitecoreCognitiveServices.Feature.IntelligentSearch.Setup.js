jQuery.noConflict();

//setup
jQuery(document).ready(function () {
    //handles setup form
    var setupForm = ".setup-form";
    jQuery(setupForm + " button")
        .click(function (event) {
            event.preventDefault();

            var luisValue = jQuery(setupForm + " #luisApi").val();
            var luisEndpointValue = jQuery(setupForm + " #luisApiEndpoint").val();
            
            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(setupForm).attr("action"),
                {
                    luisApi: luisValue,
                    luisApiEndpoint: luisEndpointValue
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(".result-failure").show();
                } else {
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
            });
        });
});