// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

var buildResourceTableRow = function buildResourceTableRow(resourceName, resourceId) {
    var parentId = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : 0;
    var isChildResource = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : false;
    var childWithoutParent = arguments.length > 4 && arguments[4] !== undefined ? arguments[4] : false;
    var editCell = "<a class=\"edit-resource-check\"><span class=\"fa fa-check action-icons\"></span></a>";
    var rowString = '<tr class="parent-resource-claim" data-state="added">';
    var iconCell = '<td class="icon-cell"></td>';
    var resourceCell = "  <td class=\"resource-label\" data-resource-id='".concat(resourceId, "'>").concat(resourceName, "</td>");

    if (isChildResource) {
        rowString = "<tr class=\"child-resource-claim\" data-state=\"added\">";
        iconCell = "<td class=\"icon-cell\"><span class=\"child-resource-branch\"></span></td>";
        resourceCell = "  <td class=\"resource-label\" data-resource-id='".concat(resourceId, "' data-parent-id='").concat(parentId, "'>").concat(resourceName, "</td>");
    } else if (!childWithoutParent) {
        iconCell = '  <td class="icon-cell"><a class="claims-toggle"><span class="fa fa-chevron-down caret-custom"></span></a></td>';
    }

    var $tableRow = $([rowString, iconCell, resourceCell, '  <td class="read-action-cell"><label><input type="checkbox" class="hide read-checkbox"><i class="fa fa-fw fa-check-square"></i></label></td>', '  <td class="create-action-cell"><label><input type="checkbox" class="hide create-checkbox"><i class="fa fa-fw fa-check-square"></i></label></td>', '  <td class="update-action-cell"><label><input type="checkbox" class="hide update-checkbox"><i class="fa fa-fw fa-check-square"></i></label></td>', '  <td class="delete-action-cell"><label><input type="checkbox" class="hide delete-checkbox"><i class="fa fa-fw fa-check-square"></i></label></td>', "  <td>".concat(editCell, "</td>"), '  <td><a class="override-auth-strategy" hidden> <span class="fa fa-info-circle action-icons"></span></a></td>', '  <td><a class="delete-resource"> <span class="fa fa-trash-o action-icons"></span></a></td>', "</tr>"].join("\n"));
    $tableRow.find(".edit-resource-check").click(saveEditedResource);
    $tableRow.find(".delete-resource").click(deleteResource);
    $tableRow.find(".claims-toggle").click(claimsToggle);
    return $tableRow;
};

var deleteResource = function deleteResource() {
    var row = $(this).closest("tr");
    var resourceId = row.find(".resource-label").attr("data-resource-id");
    enableOptionForMainResource(resourceId);
    $("tr.child-resource-claim[data-parent-id=".concat(resourceId, "]")).remove();

    if (row.hasClass("parent-resource-claim")) {
        updateChildrenAfterParentDeleted(resourceId);
    }

    enableOptionForChildResource(row);
    row[0].remove();
};

var updateCheckbox = function updateCheckbox(row, action) {
    var actionInput = row.find("input.".concat(action, "-checkbox"));

    if (actionInput.length) {
        actionInput.removeAttr("disabled");
        row.find("i").removeClass("hide");
    } else {
        var actionCellLabel = row.find("td.".concat(action, "-action-cell > label"));
        var updatedActionCellLabel = "<label><input type=\"checkbox\" class=\"hide ".concat(action, "-checkbox\"><i class=\"fa fa-fw fa-check-square\"></i></label>");
        actionCellLabel.replaceWith(updatedActionCellLabel);
    }
};

var editResource = function editResource(e) {
    e.preventDefault();
    var row = $(this).closest("tr");
    row.attr("data-state", "edited");
    updateCheckbox(row, "create");
    updateCheckbox(row, "read");
    updateCheckbox(row, "update");
    updateCheckbox(row, "delete");
    disableForEdit(row);
    row.find("a.edit-resource").replaceWith('<a class="edit-resource-check"> <span class="fa fa-check action-icons"></span></a>');
    row.find(".edit-resource-check").click(saveEditedResource);
};

var enableCellsForRow = function (editRow, cells) {
    $.each(cells, function (index, value) {
        editRow.find('td.' + value).removeClass("disabled");
    });
};

var disableForEdit = function(editRow) {
    $("#resource-claim-table-body td").addClass("disabled");

    AddTooltip($("a.edit-resource"), "You can only edit after saving the highlighted row");
    AddTooltip($("a.edit-auth-strategy"),
        "You can only override authorization strategy after saving the highlighted row");
    AddTooltip($("a.delete-resource-claim"),
        "You can only delete a resource claim after saving the highlighted row");
    AddTooltip($("a.claims-toggle"), "You can only edit after saving the highlighted row");
    AddTooltip($("a.add-child-resource-button"), "You can only add a child after saving the highlighted row");

    enableCellsForRow(editRow,
        [
            "read-action-cell", "create-action-cell", "update-action-cell", "delete-action-cell",
            "edit-resource-button", "resource-label"
        ]);

    RemoveTooltips([editRow.find("a.edit-resource")]);

    $("select.resource-claim-dropdown").attr("disabled", "disabled");
    $("td.child-dropdown>select").attr("disabled", "disabled");
};

var enableAfterEdit = function () {
    $("#resource-claim-table-body td").removeClass("disabled");

    RemoveTooltips([$("a.edit-auth-strategy"), $("a.delete-resource-claim"), $("a.claims-toggle"), $("a.add-child-resource-button")]);

    $("select.resource-claim-dropdown").attr("disabled", false);
    $("td.child-dropdown>select").attr("disabled", false);
};

var addAntiForgeryToken = function addAntiForgeryToken(data) {
    data.__RequestVerificationToken = $("#AntiForgeryToken input[name=__RequestVerificationToken]").val();
    return data;
};

var getDeleteResourceModalUrl = function getDeleteResourceModalUrl(model) {
    return "".concat(deleteResourceModalUrl, "?claimSetId=").concat(model.claimSetId, "&claimSetName=").concat(encodeURIComponent(model.claimSetName), "&resourceName=").concat(encodeURIComponent(model.resourceName), "&resourceClaimId=").concat(model.resourceId);
};

var getAuthOverrideModalUrl = function getAuthOverrideModalUrl(model) {
    return "".concat(overrideAuthStrategyModalUrl, "?claimSetId=").concat(model.claimSetId, "&resourceClaimId=").concat(model.resourceId);
};

var turnEditOffForRow = function turnEditOffForRow(row) {
    row.find("input:checkbox").each(function () {
        $(this).attr("disabled", "disabled");
        if (!($(this)[0].checked)) {
            $(this).parent().find("i").addClass("hide");
        }
    });
    row.find("a.edit-resource-check").replaceWith('<a class="edit-resource"> <span class="fa fa-pencil action-icons"></span></a>');
    row.find(".edit-resource").click(editResource);
    return row;
};

var enableOptionForChildResource = function enableOptionForChildResource(row) {
    var resourceName = row.find(".resource-label").text();
    var resourceId = row.find(".resource-label").data("resource-id");
    var parentResourceId = row.find(".resource-label").data("parent-id");

    if (parentResourceId !== undefined) {
        $("#child-resource-dropdown-".concat(parentResourceId, " option")).filter(function () {
            return $(this).html() === resourceName;
        })[0].disabled = false;
    } else {
        var optionForChildResource = $("select[name=\"ChildResourceClaimsDropDown\"] option[value=".concat(resourceId, "]"))[0];

        if (optionForChildResource !== undefined) {
            optionForChildResource.disabled = false;
        }
    }
};

var enableOptionForMainResource = function enableOptionForMainResource(resourceId) {
    var optionForResource = $("#ResourceClaimsDropDown option[value='".concat(resourceId, "']"))[0];
    optionForResource.disabled = false;
};

var disableOptionForChildResource = function disableOptionForChildResource(resourceName, parentResourceId, resourceId) {
    if (parentResourceId !== undefined && resourceName !== undefined) {
        var option = $("#child-resource-dropdown-".concat(parentResourceId, " option")).filter(function () {
            return $(this).html() === resourceName;
        })[0];

        if (option !== undefined) {
            option.disabled = true;
        }
    } else {
        var optionForChildResource = $("select[name=\"ChildResourceClaimsDropDown\"] option[value=".concat(resourceId, "]"))[0];

        if (optionForChildResource !== undefined) {
            optionForChildResource.disabled = true;
        }
    }
};

var disableOptionForMainResource = function disableOptionForMainResource(resourceId) {
    var optionForResource = $("#ResourceClaimsDropDown option[value='".concat(resourceId, "']"))[0];
    optionForResource.disabled = true;
};

var updateChildrenAfterParentDeleted = function updateChildrenAfterParentDeleted(resourceId) {
    $("td[data-parent-id=".concat(resourceId, "]")).each(function () {
        var childRow = $(this).parent();
        childRow.find("td.resource-label").removeAttr("data-parent-id");
        childRow.removeClass("child-resource-claim");
        childRow.addClass("parent-resource-claim");
        childRow.find("td.icon-cell").replaceWith('<td class="icon-cell"></td>');
    });
    $("#child-dropdown-row-".concat(resourceId)).remove();
};

var saveEditedResource = function saveEditedResource() {
    var row = $(this).closest("tr");
    var claimSetId = parseInt($("#ClaimSetId").val());
    var resourceEl = row.find(".resource-label");
    var resourceId = resourceEl.data("resource-id");
    var resourceName = resourceEl.text();
    var resource = {
        "id": resourceId,
        "name": resourceName,
        "read": row.find("input:checkbox.read-checkbox")[0].checked,
        "create": row.find("input:checkbox.create-checkbox")[0].checked,
        "update": row.find("input:checkbox.update-checkbox")[0].checked,
        "delete": row.find("input:checkbox.delete-checkbox")[0].checked
    };
    var url = editResourceUrl;
    var postData = {
        'ClaimSetId': claimSetId,
        'ResourceClaim': resource,
        'ExistingResourceClaims': existingResources
    };
    var validationBlock = $("#edit-claim-set-validation-summary")[0];
    $.ajax({
        type: "POST",
        dataType: "html",
        url: url,
        data: addAntiForgeryToken(postData),
        success: function success() {
            validationBlock.hidden = true;
            turnEditOffForRow(row);

            if (row.data("state") == "added") {
                var resourceModel = claimSetInfo;
                resourceModel.resourceName = resourceName;
                resourceModel.resourceId = resourceId;
                row.find("a.delete-resource").replaceWith("<a class=\"loads-ajax-modal\" data-url=".concat(getDeleteResourceModalUrl(resourceModel), "> <span class=\"fa fa-trash-o action-icons\"></span></a>"));
                row.find("a.override-auth-strategy").replaceWith("<a class=\"loads-ajax-modal edit-auth-strategy\" data-url=".concat(getAuthOverrideModalUrl(resourceModel), "> <span class=\"fa fa-info-circle action-icons\"></span></a>"));
                InitializeModalLoaders();
            }

            enableAfterEdit();
            disableOptionForChildResource(undefined, undefined, resourceId);
            disableAlreadyAddedResources();
            ClaimSetToastrMessage("".concat(resourceName, " edited successfully"), true);
            ClaimSetWarningMessage(true);
        },
        error: function error(data) {
            var response = JSON.parse(JSON.stringify(data));
            var errString = "";

            try {
                var responseObject = JSON.parse(response.responseText);
                var claimErrorJson = responseObject["ResourceClaim"];

                if (claimErrorJson != undefined && claimErrorJson.Errors.length > 0) {
                    $.each(claimErrorJson.Errors, function (key, value) {
                        errString = errString + value.ErrorMessage;
                    });
                }
            } catch (e) {
                errString = response.responseText;
            }

            validationBlock.hidden = false;
            validationBlock.innerText = errString;
            document.body.scrollTop = document.documentElement.scrollTop = 0;
            ClaimSetToastrMessage("There was an error in editing ".concat(resourceName));
        }
    });
};

var deleteResourceAjax = function deleteResourceAjax(e) {
    e.preventDefault();
    var modal = $("#delete-claimset-resource-modal");
    var url = deleteResourceUrl;
    var resourceId = modal.find("input[name=ResourceClaimId]")[0].value;
    var postData = {
        'ClaimSetId': modal.find("input[name=ClaimSetId]")[0].value,
        'ResourceClaimId': resourceId
    };
    var validationBlock = modal.find("#delete-resource-validation-summary")[0];
    $.ajax({
        type: "POST",
        dataType: "html",
        url: url,
        data: addAntiForgeryToken(postData),
        success: function success() {
            validationBlock.hidden = true;
            modal.modal("hide");
            var row = $("#resource-claim-table-body td[data-resource-id=".concat(resourceId, "]")).parent();

            if (row.hasClass("parent-resource-claim")) {
                updateChildrenAfterParentDeleted(resourceId);
            }

            enableOptionForChildResource(row);

            ClaimSetToastrMessage("".concat(row[0].innerText.replace(/[\n\r]+|[\s]{2,}/g, " ").trim(), " deleted successfully"), true);
            ClaimSetWarningMessage(true);

            row.remove();
            enableOptionForMainResource(resourceId);
            disableAlreadyAddedResources();
        },
        error: function error(data) {
            var response = JSON.parse(JSON.stringify(data));
            var errString = "";

            try {
                var responseObject = JSON.parse(response.responseText);
                var claimSetErrorJson = responseObject["ClaimSetId"];
                var resourceErrorJson = responseObject["ResourceClaimId"];

                if (claimSetErrorJson.Errors.length > 0) {
                    $.each(claimSetErrorJson.Errors, function (key, value) {
                        errString = errString + value.ErrorMessage;
                    });
                    errString += "\n";
                }

                if (resourceErrorJson != undefined && resourceErrorJson.Errors.length > 0) {
                    $.each(resourceErrorJson.Errors, function (key, value) {
                        errString = errString + value.ErrorMessage;
                    });
                }
            } catch (e) {
                errString = response.responseText;
            }

            validationBlock.hidden = false;
            validationBlock.innerText = errString;
            ClaimSetToastrMessage("There was an error in deleting. ".concat(errString));
        }
    });
};

var populateChildResourcesForParent = function populateChildResourcesForParent(parentResourceId) {
    $.ajax({
        type: "GET",
        url: "".concat(childResourcesForParentUrl, "?parentResourceClaimId=").concat(parentResourceId),
        contentType: "application/json; charset=utf-8",
        success: function success(data) {
            $(data).each(function () {
                var dropdown = $("#child-resource-dropdown-".concat(parentResourceId));
                var childResourceButton = $('.add-child-resource-button[data-parent-id='+parentResourceId+']');
                dropdown.change(function () {
                    childResourceButton.prop("disabled", false);
                });
                childResourceButton.click(function () {
                    childResourceButton.prop("disabled", true);
                });
                dropdown[0].selectedIndex = 0;
                dropdown.append($("<option></option>").val(this.value).html(this.text).attr("disabled", this.disabled));
                var row = $("td[data-resource-id=".concat(parentResourceId, "]")).parent();
                row.nextUntil("tr.parent-resource-claim").each(function () {
                    var resourceName = $(this).find(".resource-label").text();
                    disableOptionForChildResource(resourceName, parentResourceId);
                });
                $(".resource-label").each(function () {
                    var resourceName = $(this).text();
                    disableOptionForChildResource(resourceName, parentResourceId);
                });
            });
        }
    });
};

var claimsToggle = function claimsToggle() {
    var toggle = $(this);

    if (toggle.find("span:first").hasClass("fa-chevron-down")) {
        toggle.html('<span class="fa fa-chevron-up caret-custom"></span>');
    } else {
        toggle.html('<span class="fa fa-chevron-down caret-custom"></span>');
    }

    var row = $(this).closest(".parent-resource-claim");
    row.nextUntil("tr.parent-resource-claim").slideToggle(100, function () { });
};

var buildChildDropdownRow = function buildChildDropdownRow(parentResourceId) {
    var $row = $(["<tr class=\"child-resource-claim\" data-parent-id='".concat(parentResourceId, "' id='child-dropdown-row-").concat(parentResourceId, "' style=\"display: none\">"), "<td class=\"icon-cell\"></td>", "<td class=\"child-dropdown\"><select id=\"child-resource-dropdown-".concat(parentResourceId, "\" name=\"ChildResourceClaimsDropDown\"></select></td>"), "<td colspan=\"6\"><button type=\"button\" disabled class=\"btn btn-primary cta add-child-resource-button\" data-parent-id=\"".concat(parentResourceId, "\">Add Child Resource</button></td>"), "</tr>"].join("\n"));
    $row.find(".add-child-resource-button").click(addChildResourceButton);
    return $row;
};

$("#add-resource-button").click(function (e) {        
    e.preventDefault();
    var dropDownList = $("#ResourceClaimsDropDown")[0];    
    var selectedItem = dropDownList.options[dropDownList.selectedIndex];
    if (selectedItem.closest("optgroup")) {        
        var optGroup = selectedItem.closest("optgroup").label;
        if (!selectedItem.disabled) {
            if (optGroup === "Groups") {
                $("#resource-claim-table-body").append(buildResourceTableRow(selectedItem.text, selectedItem.value));
                $("#resource-claim-table-body").append(buildChildDropdownRow(selectedItem.value));
                populateChildResourcesForParent(selectedItem.value);
            } else {
                $("#resource-claim-table-body").append(buildResourceTableRow(selectedItem.text, selectedItem.value, 0, false, true));
                disableOptionForChildResource(undefined, undefined, selectedItem.value);
            }
        }
        selectedItem.disabled = true;
        dropDownList.selectedIndex = 0;
    }
});

var addChildResourceButton = function addChildResourceButton(e) {
    e.preventDefault();
    var parentResourceId = $(this).data('parent-id');
    var row = $("#child-dropdown-row-".concat(parentResourceId));
    var dropDownList = row.find("select[name=\"ChildResourceClaimsDropDown\"]")[0];
    var selectedItem = dropDownList.options[dropDownList.selectedIndex];

    if (selectedItem != undefined) {
        if (!selectedItem.disabled) {
            var newRow = buildResourceTableRow(selectedItem.text, selectedItem.value, row.data("parent-id"), true);
            newRow.insertBefore(row);
        }

        selectedItem.disabled = true;
        dropDownList.selectedIndex = 0;
        disableOptionForMainResource(selectedItem.value);
    }
};

$(".add-child-resource-button").click(addChildResourceButton);

var disableAlreadyAddedResources = function disableAlreadyAddedResources() {
    $(".resource-label").each(function () {
        var resourceId = $(this).data('resource-id');
        disableOptionForMainResource(resourceId);
    });
};

var populateChildren = function populateChildren() {
    $(".claims-toggle").each(function () {
        var resourceId = $(this).data("resource-id");
        populateChildResourcesForParent(resourceId);
    });
};

$(function () {
    InitializeModalLoaders();
    $(".edit-resource").click(editResource);
    var dropDownList = $("#ResourceClaimsDropDown")[0];
    dropDownList.removeAttribute("name");
    populateChildren();
    disableAlreadyAddedResources();
    $(".claims-toggle").click(claimsToggle);
});
