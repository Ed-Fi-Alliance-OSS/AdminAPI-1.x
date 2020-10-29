// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

var submitAjax = function ($form, updateTargetId, spinnerId) {
    var $allSubmitButtons = $("[type='submit']");
    $allSubmitButtons.prop("disabled", true);

    $form.find("div").removeClass("has-error");

    $.ajax({
        url: $form.attr("action"),
        type: "post",
        data: $form.serialize(),
        beforeSend: function (xhr) {
            xhr.setRequestHeader("RequestVerificationToken", getAntiForgeryToken());
        },
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        global: false,  //global handlers disabled as logic below handles these cases
        success: function (data, status, xhr) {

            $.redirectIfAuthenticationRequired(xhr, data);
            if ($form.data("tab-claimset")) {
                SetWarningTimer();
                UpdateWarningTimer();
            }
            if (!StringIsNullOrWhitespace(updateTargetId)) {
                var $updateTarget = $("#" + updateTargetId);
                $updateTarget.html(xhr.responseText);
                if ($updateTarget.hasClass("modal-content")) {
                    ReinitializeForModal();
                } else {
                    GlobalInitialize();
                }
            } else if (!StringIsNullOrWhitespace(data.redirect)) {
                SetSuccessMessage(data.successMessage);
                SetErrorMessage(data.errorMessage);
                window.location = data.redirect;
            } else {
                SetSuccessMessage(data.successMessage);
                SetErrorMessage(data.errorMessage);
                window.location.reload();
            }
        },
        complete: function() {
            $allSubmitButtons.prop("disabled", false);
            if (spinnerId) {
                $('#' + spinnerId).addClass("invisible");
            }
        },
        error: function (xhr) {
            var $validationSummary = $form.find(".validationSummary").first();
            errorHighlighter(xhr, $validationSummary);
        }
    });
}

var submitAjaxForInnerTabs = function (ajaxRequestData) {
    var form = ajaxRequestData.form;
    var url = form.attr('action');
    var validationBlock = form.find('.validationSummary').first()[0];
    var successHandler;
    if (ajaxRequestData.successHandler != null) {
        successHandler = ajaxRequestData.successHandler;
    } else {
        successHandler = function(data) {
            if (ajaxRequestData.successAction != null) {
                var $panel = $("#" + ajaxRequestData.panelId);
                history.pushState({ state: ajaxRequestData.successAction.state },
                    "",
                    ajaxRequestData.successAction.url);
                $panel.html(data);
            }
            validationBlock.hidden = true;
            if (ajaxRequestData.successAdditionalBehavior != null)
                ajaxRequestData.successAdditionalBehavior();
        };
    }

    var errorHandler;
    if (ajaxRequestData.errorHandler != null) {
        errorHandler = ajaxRequestData.errorHandler;
    } else {
        errorHandler = function (data) {
            validationBlock.hidden = false;
            validationBlock.innerText = getErrorString(data);
            if (ajaxRequestData.errorAdditionalBehavior != null)
                ajaxRequestData.errorAdditionalBehavior();
        };
    }
    $.ajax({
        type: "POST",
        url: url,
        dataType: ajaxRequestData.dataType,
        data: ajaxRequestData.formData,
        cache: ajaxRequestData.cache,
        contentType: ajaxRequestData.contentType,
        processData: ajaxRequestData.processData,
        success: successHandler,
        error: errorHandler
    });
}

var getErrorString = function(data) {
    var errString = "";
    var errorObject;
    try {
        if (data.responseJSON != null) {
            errorObject = data.responseJSON;
        } else {
            errorObject = JSON.parse(data.responseText);
        }
        $.each(errorObject,
            function(_ , errorField) {
                var errorJson = errorField;
                if (errorJson.Errors.length > 0) {
                    $.each(errorJson.Errors,
                        function(key, value) {
                            errString = errString + value.ErrorMessage + "\n";
                        });
                }
            }
        );
    } catch (e) {
        errString = data.responseText;
    }
    return errString;
}

var errorHighlighter = function (xhr, $validationSummary) {
    if (xhr.status === 400) {
        var data = JSON.parse(xhr.responseText);
        highlightFields(data);
        showSummary(data, $validationSummary);
    } else {
        var errorMessage = "An error occured while processing your request: ";
        if (!StringIsNullOrWhitespace(xhr.responseText)) {
            errorMessage = errorMessage + xhr.responseText + ". ";
        }
        if (xhr.status > 0) {
            errorMessage = errorMessage + "Status " + xhr.status;
        } else {
            errorMessage = errorMessage + "No response from server";
        }
        showError(errorMessage, $validationSummary);
    }
}

var showError = function (errorMessage, $validationSummary) {
    var $ul = $("<ul></ul>");
    $validationSummary.append($ul);

    var $li = $("<li></li>").text(errorMessage);
    $li.appendTo($ul);

    $validationSummary.removeClass("hidden");
    $validationSummary.show();
}

var highlightFields = function (response) {
    $(".form-group").removeClass("has-error");

    $.each(response, function (propName, val) {
        var nameSelector = '[name = "' + propName.replace(/(:|\.|\[|\])/g, "\\$1") + '"]';
        var idSelector = '#' + propName.replace(/(:|\.|\[|\])/g, "\\$1");
        var element = $(nameSelector) || $(idSelector);

        if (val.Errors.length > 0) {
            element.closest(".form-group").addClass("has-error");
        }
    });
};

var showSummary = function (response, $validationSummary) {
    $validationSummary.empty();
    var verboseErrors = _.flatten(_.map(response, "Errors"));
    var errors = [];

    var nonNullErrors = _.reject(verboseErrors, function (error) {
        return error.ErrorMessage.indexOf("must not be empty") > -1;
    });

    _.each(nonNullErrors, function (error) {
        errors.push(error.ErrorMessage);
    });

    if (nonNullErrors.length !== verboseErrors.length) {
        errors.push("The highlighted fields are required to submit this form.");
    }

    var $ul = $("<ul></ul>");
    $validationSummary.append($ul);

    _.each(errors, function (error) {
        var $li = $("<li></li>").text(error);
        $li.appendTo($ul);
    });

    $validationSummary.removeClass("hidden");
    $validationSummary.show();
};

var navigationAjax = function (navigationalUrl, panel, state, pageUrl, action) {
    var url = navigationalUrl;
    $.ajax({
        url: url,
        success: function (data) {
            history.pushState({ state: state }, "", pageUrl);
            panel.html(data);
        },
        error: function () {
            var errorMessage = "An error occured while " + action;
            panel.html("<em class='text-danger margin-top'>"+errorMessage+"</em>");
        }
    });
}

var backNavigationAjax = function (navigationalUrl, isClaimSetTab = false) {
    var url = navigationalUrl;
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            history.replaceState({ state: 'listing' }, "", url);
            if (isClaimSetTab) {
                $("#global-tab").html($(data).find('#global-tab').html());
                if (sessionStorage.getItem("LatestDatabaseChangeTimestamp") != null) {
                    $("#claim-set-warning-message").show();
                }
            } else {
                $("section").html($(data).find('section').html());
            }
        }
    });
}

function getAntiForgeryToken() {
    return $("#AntiForgeryToken").find("input[name=__RequestVerificationToken]").val();
}

function appendAntiForgeryToken(serializedForm) {
    var antiForgeryTokenValue = getAntiForgeryToken();
    return serializedForm + "&__RequestVerificationToken=" + antiForgeryTokenValue;
};

function appendAntiForgeryTokenForFileUpload($form) {
    var antiForgeryTokenValue = getAntiForgeryToken();
    var formData = new FormData($form[0]);
    formData.append("__RequestVerificationToken", antiForgeryTokenValue);

    return formData;
};
