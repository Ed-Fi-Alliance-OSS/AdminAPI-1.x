// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

var updateCell = function(row, action) {
    var resourceId = row.find(".resource-label").data("resource-id");
    var actionCell = row.find("td.".concat(action, "-action-cell"));
    var actionCellLabel = actionCell.find("span:first-child");
    var authStrategyLabel = actionCell.find("span:nth-child(2)");
    var dropdown = '';
    if (actionCell.data("existing-action") === "True") {
        dropdown = $("<select class='auth-dropdown' id='resource-auth-dropdown-".concat(resourceId, "-", action, "'></select>"));
        $(authStrategiesOptions).each(function () {
            if (this.Text === actionCellLabel.data('default-strategy')) {
                if (actionCellLabel.data('is-inherited') === "True") {
                    dropdown.append($("<option></option>").val(this.Value).html(this.Text.concat("(Default Strategy)")).attr("disabled", this.Disabled).attr("data-default-is-inherited", true));
                } else {
                    dropdown.append($("<option></option>").val(this.Value).html(this.Text.concat("(Default Strategy)")).attr("disabled", this.Disabled));
                }
            } else {
                dropdown.append($("<option></option>").val(this.Value).html(this.Text).attr("disabled", this.Disabled));
            }
        });
        var selectedValue = dropdown.find('option:contains('.concat(actionCellLabel.text(), ')'))[0].value;
        dropdown.val(selectedValue);
        actionCellLabel.html(dropdown);
        authStrategyLabel.html('');
    }
};

var getUpdateResourceUrl = function (resourceId, claimSetId) {
    return "".concat(getUpdatedResourceUrl, "?claimSetId=").concat(claimSetId, "&resourceClaimId=").concat(resourceId);
};

var showSpinner= function(shouldShow) {
    if (shouldShow) {
        $("#auth-modal-spinner").removeClass("hidden");
        $("#auth-table").addClass("hidden");
    } else {
        $("#auth-modal-spinner").addClass("hidden");
        $("#auth-table").removeClass("hidden");
    }
}

var updateRowAfterEdit = function (row, resourceUpdateUrl) {
    $.ajax({
        type: "GET",
        dataType: "json",
        url: resourceUpdateUrl,
        success: function success(updatedResource) {
            var defaultStrategies = updatedResource.DefaultAuthStrategiesForCRUD;
            var authStrategyOverrides = updatedResource.AuthStrategyOverridesForCRUD;
            var readCell = row.find(".read-action-cell");
            var createCell = row.find(".create-action-cell");
            var updateCell = row.find(".update-action-cell");
            var deleteCell = row.find(".delete-action-cell");
            var editCell = row.find("a.edit-resource-check");
            updateCellAfterEdit(readCell, defaultStrategies[1], authStrategyOverrides[1]);
            updateCellAfterEdit(createCell, defaultStrategies[0], authStrategyOverrides[0]);
            updateCellAfterEdit(updateCell, defaultStrategies[2], authStrategyOverrides[2]);
            updateCellAfterEdit(deleteCell, defaultStrategies[3], authStrategyOverrides[3]);
            if (editCell != null) {
                row.find("a.edit-resource-check").replaceWith('<a class="override-auth"> <span class="fa fa-pencil action-icons"></span></a>');
                row.find(".override-auth").click(overrideAuth);
            }
            showSpinner(false);
        },
        error: function error() {
            showSpinner(false);
        }
    });
};

var updateCellAfterEdit = function (cell, defaultStrategy, authStrategyOverride) {
    var strategyName = cell.find("span:first-child");
    if (authStrategyOverride != null) {
        strategyName.html(authStrategyOverride.DisplayName);
        if (authStrategyOverride.IsInheritedFromParent) {
            cell.find("span:nth-child(2)")
                .replaceWith('<span class="overridden-strategy inherited-override">(Overridden)</span>');
        } else {
            cell.find("span:nth-child(2)")
                .replaceWith('<span class="overridden-strategy">(Overridden)</span>');
        }
    } else {
        strategyName.html(defaultStrategy.DisplayName);
        if (defaultStrategy.IsInheritedFromParent) {
            cell.find("span:nth-child(2)")
                .replaceWith('<span class="default-strategy inherited-strategy">(Default)</span>');
        } else {
            cell.find("span:nth-child(2)")
                .replaceWith('<span class="default-strategy">(Default)</span>');
        }
    }
}

var overrideStrategies = function () {
    var row = $(this).closest("tr");
    var claimSetId = parseInt($("#ClaimSetId").val());
    var resourceEl = row.find(".resource-label");
    var resourceId = resourceEl.data("resource-id");
    var resourceName = resourceEl.text();
    var readDropdown = row.find(".read-action-cell select");
    var createDropdown = row.find(".create-action-cell select");
    var updateDropdown = row.find(".update-action-cell select");
    var deleteDropdown = row.find(".delete-action-cell select");
    var isReadSelectedOptionDefault = readDropdown.find('option:selected').text().trim().includes('(Default Strategy)');
    var isCreateSelectedOptionDefault = createDropdown.find('option:selected').text().trim().includes('(Default Strategy)');
    var isUpdateSelectedOptionDefault = updateDropdown.find('option:selected').text().trim().includes('(Default Strategy)');
    var isDeleteSelectedOptionDefault = deleteDropdown.find('option:selected').text().trim().includes('(Default Strategy)');
    var postData = {
        'ClaimSetId': claimSetId,
        'ResourceClaimId': resourceId,
        'AuthorizationStrategyForCreate': isCreateSelectedOptionDefault ? 0 : createDropdown.val(),
        'AuthorizationStrategyForRead': isReadSelectedOptionDefault ? 0 : readDropdown.val(),
        'AuthorizationStrategyForUpdate': isUpdateSelectedOptionDefault ? 0 : updateDropdown.val(),
        'AuthorizationStrategyForDelete': isDeleteSelectedOptionDefault ? 0 : deleteDropdown.val()
    };
    showSpinner(true);
    $.ajax({
        type: "POST",
        dataType: "html",
        url: authStrategyOverrideUrl,
        data: addAntiForgeryToken(postData),
        success: function success() {
            resourceUpdateUrl = getUpdateResourceUrl(resourceId, claimSetId);
            updateRowAfterEdit(row, resourceUpdateUrl);
            ClaimSetToastrMessage("".concat(resourceName, " authorization strategies edited successfully"), true);
            ClaimSetWarningMessage(true);
        },
        error: function error(data) {
            document.body.scrollTop = document.documentElement.scrollTop = 0;
            showSpinner(false);
            ClaimSetToastrMessage("There was an error in overriding strategies on ".concat(resourceName).concat(data.responseText));
        }
    });
};

var resetStrategiesToDefault = function () {
    var row = $(this).closest("tr");
    var claimSetId = parseInt($("#ClaimSetId").val());
    var resourceEl = row.find(".resource-label");
    var resourceId = resourceEl.data("resource-id");
    var resourceName = resourceEl.text();
    var readCell = row.find(".read-action-cell");
    var createCell = row.find(".create-action-cell");
    var updateCell = row.find(".update-action-cell");
    var deleteCell = row.find(".delete-action-cell");
    var postData = {
        'ClaimSetId': claimSetId,
        'ResourceClaimId': resourceId
    };
    if (!(readCell.find("span:nth-child(2)").hasClass("default-strategy") &&
        createCell.find("span:nth-child(2)").hasClass("default-strategy") &&
        updateCell.find("span:nth-child(2)").hasClass("default-strategy") &&
        deleteCell.find("span:nth-child(2)").hasClass("default-strategy"))) {
        showSpinner(true);
        $.ajax({
            type: "POST",
            dataType: "html",
            url: resetAuthStrategyUrl,
            data: addAntiForgeryToken(postData),
            success: function success() {
                resourceUpdateUrl = getUpdateResourceUrl(resourceId, claimSetId);
                updateRowAfterEdit(row, resourceUpdateUrl);
                ClaimSetToastrMessage("".concat(resourceName, " authorization strategies reset to default successfully"), true);
                ClaimSetWarningMessage(true);
            },
            error: function error(data) {
                document.body.scrollTop = document.documentElement.scrollTop = 0;
                showSpinner(false);
                ClaimSetToastrMessage("There was an error in resetting overrides on ".concat(resourceName).concat(data.responseText));
            }
        });
    }
};

var overrideAuth = function(e) {
    e.preventDefault();
    var row = $(this).closest("tr");
    row.attr("data-state", "edited");
    updateCell(row, "create");
    updateCell(row, "read");
    updateCell(row, "update");
    updateCell(row, "delete");
    row.find("a.override-auth").replaceWith('<a class="edit-resource-check"> <span class="fa fa-check action-icons"></span></a>');
    $(".edit-resource-check").click(overrideStrategies);
};
$(function () {
    $(".override-auth").click(overrideAuth);
    $(".reset-auth").click(resetStrategiesToDefault);
});
