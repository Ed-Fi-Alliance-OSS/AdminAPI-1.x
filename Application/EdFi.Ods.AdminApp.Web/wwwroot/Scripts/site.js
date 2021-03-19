// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

var edfiODS = edfiODS || {};

$.extend(true, edfiODS, {
    httpStatusCodes: {
        OK: 200,
        Accepted: 202,
        NoContent: 204,
        BadRequest: 400
    }
});

$.extend({
    redirectIfAuthenticationRequired: function(xhr, data) {
        if (data.authentication_required || (typeof data === "string" && data.indexOf("authentication_required") !== -1) || xhr.getResponseHeader("x-authentication-required")) {
            window.location.reload(); //re-trigger authentication workflow
        }
    }
});

(function ($) {
    //none of our AJAX calls should be cached to ensure modals always load with correct data
    $.ajaxSetup({ cache: false });

    $(document).ajaxSuccess(function (event, xhr, settings, data) {
        $.redirectIfAuthenticationRequired(xhr, data);
    });
})(jQuery);

function GlobalInitialize() {
    //note that this function (and any it calls) must be idempotent
    //as it may be called more than once and should not double-register events
    InitializeHelpTooltips();
    InitializeSubmitButtons();
    InitializeAjaxPostButtons();
    InitializeModalLoaders();
    InitializeSelectLists();
    InitializeVisibilityToggles();
    ShowToasterMessage();
    ShowServerToastMessage();
    ClearToasterMessage();
    SetupPanelToggle();
    LoadAsyncActions();
    ClaimSetWarningMessage();
    AttachDefaultPagingBehavior();
}

function ReinitializeForModal() {
    InitializeSubmitButtons();
    InitializeHelpTooltips();
    InitializeSelectLists();
    InitializeVisibilityToggles();
}

function InitializeHelpTooltips() {
    var $tooltips = $("[data-toggle='tooltip']");
    $tooltips.tooltip("destroy");

    $tooltips.tooltip(
    {
        placement: "auto right",
        container: "body"
    });
}

function InitializeAjaxPostButtons() {
    var $ajaxButtons = $('.ajax-button');
    $ajaxButtons.off('click');

    $ajaxButtons.each(function () {
        var $button = $(this);
        $button.off('click');

        $button.click(function (e) {
            e.preventDefault();

            var button = $(this);

            $.ajax({
                url: button.attr('href'),
                type: 'POST',
                data: { "__RequestVerificationToken": getAntiForgeryToken() },
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                global: false,
                success: function (data, status, xhr) {
                    $.redirectIfAuthenticationRequired(xhr, data);

                    if (!StringIsNullOrWhitespace(data.redirect)) {
                        SetSuccessMessage(data.successMessage);
                        SetErrorMessage(data.errorMessage);
                        window.location = data.redirect;
                    } else {
                        SetSuccessMessage(data.successMessage);
                        SetErrorMessage(data.errorMessage);
                        window.location.reload();
                    }
                },
                error: function (data) {
                    SetErrorMessage(data.SetErrorMessage);
                }
            });
        });
    });
}

function InitializeSubmitButtons() {
    var $submitButtons = $("[type='submit']").not(".no-ajax");
    $submitButtons.off("click");

    $submitButtons.each(function() {
        var $button = $(this);
        $button.off("click");
        $button.click(function(event) {
            event.preventDefault();
            var $form = $button.closest("div:has(form)").find("form");
            var updateTargetId = $button.data("update-target-id");

            var spinnerId = $button.data("spinner-id");
            if (spinnerId) {
                $("#" + spinnerId).removeClass("invisible");
            }

            submitAjax($form, updateTargetId, spinnerId);

            return false;
        });
    });
}

function InitializeNavigationalAjaxButtons() {
    var $navigationalAjaxButtons = $('.navigational-ajax');
    $navigationalAjaxButtons.off("click");

    $navigationalAjaxButtons.each(function() {
        var $button = $(this);
        $button.off("click");
        $button.click(function(event) {
            event.preventDefault();
            var url = $(this).attr("href");
            var panel = $('.navigational-index');
            var pageUrl = $(this).data('page-url');
            var state = $(this).data('page-url');
            var action = $(this).data('action');
            navigationAjax(url, panel, state, pageUrl, action);

            return false;
        });
    });
}

function InitializeBackNavigationalAjaxButtons() {
    var $backAjaxButtons = $('.back-btn.back-ajax');
    $backAjaxButtons.off("click");

    $backAjaxButtons.each(function () {
        var $button = $(this);
        $button.off("click");
        $button.click(function (event) {
            var url = $(this).data('back-url');
            var isClaimSetTab = false;
            if ($(this).hasClass("claimset-back-btn"))
                isClaimSetTab = true;
            backNavigationAjax(url, isClaimSetTab);

            return false;
        });
    });
}

function InitializeModalLoaders() {
    $(".loads-ajax-modal").off("click");
    $(".loads-ajax-modal").click(function (e) {
        e.preventDefault();
        $("body").css("cursor", "progress");
        var $button = $(this);
        $button.addClass("loading-modal");

        var url = $(this).data("url");
        $.ajax({
            type: "GET",
            url: url,
            success: function (data) {
                if (!data.authentication_required) {
                    $("#dynamic-modal-container").html(data);

                    ReinitializeForModal();

                    var dynamicModal = $("#dynamic-modal-container").find("div.modal");
                    if (!dynamicModal)
                        return;

                    dynamicModal.on('hidden.bs.modal',
                        function (e) {
                            $('#dynamic-modal-container').empty();
                        });

                    $button.removeClass("loading-modal");
                    $("body").css("cursor", "auto");

                    dynamicModal.modal({
                        backdrop: 'static',
                        keyboard: false
                    });
                }
            }
        });
    });
}

function InitializeSelectLists() {
    $(document).ready(function () {
        $('select[multiple="multiple"]').multiselect({
            numberDisplayed: 2,
            enableCaseInsensitiveFiltering: true,
            filterPlaceholder: 'Search',
            maxHeight: 500,
            onDropdownHide: function () {
                $('button.multiselect-clear-filter').click();
            }
        });
    });
}

function InitializeVisibilityToggles() {
    $(".hides-elements").off("click");
    $(".shows-elements").off("click");

    $(".hides-elements").click(function () {
        var associatedElements = $(this).data("associated-elements");
        var associatedClassId = "." + associatedElements;
        $(associatedClassId).hide();

        var toggleSelector = '.shows-elements[data-associated-elements="' + associatedElements + '"]';
        $(this).hide();
        $(toggleSelector).show();
    });

    $(".shows-elements").click(function () {
        var associatedElements = $(this).data("associated-elements");
        var associatedClassId = "." + associatedElements;
        $(associatedClassId).show();

        var toggleSelector = '.hides-elements[data-associated-elements="' + associatedElements + '"]';
        $(this).hide();
        $(toggleSelector).show();
    });
}

function ShowToasterMessage() {
    var successMessage = localStorage.getItem("SuccessMessage");
    if (!StringIsNullOrWhitespace(successMessage)) {
        toastr.success(successMessage);
    }

    var errorMessage = localStorage.getItem("ErrorMessage");
    if (!StringIsNullOrWhitespace(errorMessage)) {
        toastr.error(errorMessage);
    }
}

function ShowServerToastMessage() {
    $("#toast").each(function () {
        var $toast = $(this);
        var type = $toast.data("type");
        var message = $toast.val();
        toastr[type](message);
    });
}

function InitializeCopyToClipboardLinks() {
    if (!Clipboard.isSupported()) {
        var $copyToClipboardLinks = $(".copy-to-clipboard");
        $copyToClipboardLinks.find(".fa-clipboard").hide();
        $copyToClipboardLinks.removeAttr("href");
        return;
    }

    var clipboardCopy = new Clipboard(".copy-to-clipboard");
    clipboardCopy.on("success", function (e) {
        var successMessage = "Copied to clipboard!";
        var successHtml = "<span class='fa fa-check'></span><span>" + successMessage + "</span>";
        var $link = $(e.trigger);
        var currentContent = $link.html();

        if (currentContent.includes(successMessage)) {
            return;
        }

        $link.html(successHtml);
        setTimeout(function () { $link.html(currentContent) }, 2500);
    });
}

function ClearToasterMessage() {
    localStorage.removeItem("SuccessMessage");
    localStorage.removeItem("ErrorMessage");
}

function StringIsNullOrWhitespace(string) {

    if (typeof string === "undefined" || string === null) return true;

    return string.replace(/\s/g, "").length < 1;
}

function SetSuccessMessage(message)
{
    if (!StringIsNullOrWhitespace(message)) {
        localStorage.setItem("SuccessMessage", message);
    }
}

function SetErrorMessage(message) {
    if (!StringIsNullOrWhitespace(message)) {
        localStorage.setItem("ErrorMessage", message);
    }
}

function ShowNewLoadingOverlay($containerElement, overlayId)
{
    if ($("#" + overlayId).length > 0) {
        return;
    }

    var $overlay = $("#loading-overlay").clone().addClass("overlay").attr("id", overlayId);
    var containerOffsetEdges = $containerElement.offset();
    $overlay.css({ top: containerOffsetEdges.top, left: containerOffsetEdges.left, width: $containerElement.outerWidth(), height: $containerElement.outerHeight() });
    $overlay.prependTo("body");
}

function DestroyLoadingOverlay(overlayId) {
    $("#" + overlayId).remove();
}

function DestroyAllLoadingOverlays() {
    $(".overlay").remove();
}

function SetupPanelToggle() {
    $(".panel-toggle").off("click");
    $(".panel-toggle").click(function (e) {
        var $toggleIcon = $(this);
        if ($toggleIcon.hasClass("fa-chevron-up")) {
            $toggleIcon.removeClass("fa-chevron-up");
            $toggleIcon.addClass("fa-chevron-down");
        } else {
            $toggleIcon.removeClass("fa-chevron-down");
            $toggleIcon.addClass("fa-chevron-up");
        }
    });
}

function LoadAsyncActions() {
    $(".load-action-async").each(function () {
        var $target = $(this);
        var sourceUrl = $target.attr("data-source-url");
        var customErrorMessage = $target.attr("data-error-message");

        $.ajax({
            async: true,
            type: "GET",
            url: sourceUrl,
            success: function (data) {
                $target.html(data);
            },
            error: function (jqXhr) {
                var errorMessage = "The following error occured while loading page content: ";
                if (!StringIsNullOrWhitespace(jqXhr.responseText)) {
                    errorMessage = errorMessage + jqXhr.responseText + ". ";
                }

                if (jqXhr.status > 0) {
                    errorMessage = errorMessage + "Status " + jqXhr.status;
                } else {
                    errorMessage = errorMessage + "No response from server";
                }

                if (!StringIsNullOrWhitespace(customErrorMessage)) {
                    errorMessage = errorMessage + ". <br/><b>" + customErrorMessage + "</b>";
                }

                $target.html("<em class='text-danger'>" + errorMessage + "</em>");
            },
            complete: function() {
                $target.removeClass("load-action-async");
                $target.removeAttr("style");
                GlobalInitialize();
            }
        });
    });
}

function SetWarningTimer() {
    sessionStorage.setItem("LatestDatabaseChangeTimestamp", (new Date().getTime()));
}

function GetWarningTimer() {
    if (typeof cacheTimeout === 'undefined') {
        return 0;
    }
    var cacheRefreshTimeInMinutes = parseInt(cacheTimeout);
    var timeOfNextRefresh = null;
    if (cacheRefreshTimeInMinutes !== 0) {
        timeOfNextRefresh = parseInt(sessionStorage.getItem("LatestDatabaseChangeTimestamp")) +
            cacheRefreshTimeInMinutes * 1000 * 60;
    }
    return timeOfNextRefresh;
}

function ClaimSetWarningMessage(shouldTriggerTimer = false) {
    if (shouldTriggerTimer) {
        SetWarningTimer();
    }
    if (sessionStorage.getItem("LatestDatabaseChangeTimestamp") != null) {
        UpdateWarningTimer();

        var warningMessageDiv = $("#claim-set-warning-message");
        if (warningMessageDiv != null) {
            warningMessageDiv.show();
            if (warningMessageDiv.offset() != undefined) {
                $("html, body").animate({ scrollTop: warningMessageDiv.offset().top }, "fast");
            }
        }
    }
}

function ClaimSetToastrMessage(message) {
    var isSuccess = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : false;

    if (isSuccess) {
        SetSuccessMessage(message);
    } else {
        SetErrorMessage(message);
    }

    ShowToasterMessage();
    ClearToasterMessage();
}

function UpdateWarningTimer() {
    var warningTimerSpan = $("#claim-set-warning-time");
    var warningMessageDiv = $("#claim-set-warning-message");
    var timeOfNextRefresh = GetWarningTimer();
    if (timeOfNextRefresh != null) {
        var timeToNextRefresh = timeOfNextRefresh - (new Date().getTime());
        if (timeToNextRefresh < 0) {
            warningMessageDiv.hide();
            warningTimerSpan.innerHtml = "";
            sessionStorage.removeItem("LatestDatabaseChangeTimestamp");
        } else {
            var minutesToRefresh = Math.floor((timeToNextRefresh % (1000 * 60 * 60)) / (1000 * 60));
            var secondsToRefresh = Math.floor((timeToNextRefresh % (1000 * 60)) / 1000);
            var secondsString = secondsToRefresh;
            if (secondsToRefresh < 10) {
                secondsString = "0" + secondsToRefresh;
            }
            warningTimerSpan.text(minutesToRefresh + ":" + secondsString);
            setTimeout(UpdateWarningTimer, 1000);
        }
    } else {
        $("#claim-set-warning-message").find("p").html("<strong>Please restart the ODS / API for the latest claim set changes to take effect.</strong>");
    }
};

var replacePagedContent = function (event) {
    event.preventDefault();

    var $btn = $(this),
        url = $btn.attr('href');

    $.ajax(url).done(function (data) {
        $btn.closest('.ajax-content').html(data);
        GlobalInitialize();
    });
};

function AttachDefaultPagingBehavior() {
    $('.navigate-previous-page').off("click");
    $('.navigate-previous-page').on("click", replacePagedContent);

    $('.navigate-next-page').off("click");
    $('.navigate-next-page').on("click", replacePagedContent);
};   

function AddTooltip(element, tooltipMessage) {
    element.attr("data-toggle", "tooltip");
    element.attr("title", tooltipMessage);
};

function RemoveTooltips(elements) {
    $.each(elements, function (index, element) {
        element.removeAttr("data-toggle").removeAttr("title");
    });
};
