var menuDataSrc = null;
var parentMenuSelect = null;
var categoryId = null;
var perentId = null;
var manufacturerId = null;
var topicId = null;
//var categoryMenuSelect = null;
//var categoryList = null;
$(document).ready(function () {
    menuDataSrc = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: "/MenuManager/GetMenusTv",
                dataType: "json",
            }
        },
        schema: {
            model: {
                id: "Id",
                children: "ChildMenus",
                hasChildren: "HasChildren"
            }
        }
    });

    $("#Menus-tree").kendoTreeView({
        dataSource: menuDataSrc,
        dragAndDrop: true,
        dataTextField: "Title",
        select: onTreeSelect,
        drop: onTreeDrop
    });

    parentMenuSelect = $("#ParentMenuId").kendoComboBox({
        placeholder: "Select Parent Menu",
        dataTextField: "Title",
        dataValueField: "Id",
        filter: "contains",
        autoBind: true,
        minLength: 3,
        dataSource: menuDataSrc
    }).data("kendoComboBox");

    if ($("#menusmpl").length > 0) {
        $("#menusmpl").kendoMenu({
            //dataSource: dataSrc,
            dataTextField: "Title"
        });
    }

    $("#StartDate").kendoDatePicker();
    $("#EndDate").kendoDatePicker();
    $("#MenuOrder").kendoNumericTextBox();
    $("#btnSaveTv").bind("click", SaveTreeChanges);

    $('#form0').submit(function (e) {
        return chkfrm();
    });
    $('#frmReset').click(function (e) {
        $('#form0')[0].reset();
        $('#hid').val('');
        $('#Title').val('');
    });
});

function chkfrm() {
    var hasUrl = false, hasCntrllrAction = false, hasMenuName = false, htsMenuTitle = false;
    var fld = $('#MenuName');
    if (fld.val() == '') {
        makered('#MenuName');
        hasMenuName = false;
    }
    else {
        remred('#MenuName');
        hasMenuName = true;
    }
    fld = $('#MenuTitle');
    if (fld.val() == '') {
        // alert('Please enter your Name.');
        makered('#MenuTitle');
        htsMenuTitle = false;
    }
    else {
        remred('#MenuTitle');
        htsMenuTitle = true;
    }
    if ($('#PermanentRedirect').is('checked')) {
        if ($('#PermanentRedirectUrl').val() == '') {
            makered('#PermanentRedirectUrl');
        }
        else
            hasUrl = true;
    }
    else {
        remred('#PermanentRedirectUrl');
        hasUrl = true;
    }

    if ((($('#MenuController').val() != '') && ($('#MenuControllerAction').val() == '')) ||
        (($('#MenuControllerAction').val() != '') && ($('#MenuController').val() == ''))) {
        makered('#MenuController');
        makered('#MenuControllerAction');

        hasCntrllrAction = false;
    }
    else {
        hasCntrllrAction = true;
    }
    return hasMenuName && htsMenuTitle && hasCntrllrAction;
}

function makered(v) {
    if ($(v).length > 0) { $(v).css('border-color', '#B70000'); }
}
function remred(v) {
    if ($(v).length > 0) { $(v).removeAttr('style'); }
}
function pgfrmSuccess() {
    $("#frmMailmessage").hide();
    $("#lblMessage").show();
}
function onFormSuccess(data, status, xhr) {
    menuDataSrc.read();
    $('#form0')[0].reset();
    $('#hid').val('');
}

function onFormFailure(xhr, status, error) {
    //alert("Whoops! That didn't go so well did it?");
}

function onTreeSelect(e) {
    var dataItem = this.dataItem(e.node);
    var uri = "/MenuManager/GetMenu/";
    $.ajax({
        url: uri,
        data: { id: dataItem.Id },
        dataType: "json",
        context: document.body
    }).success(function (data) {

      pmenuDataSrc = new kendo.data.HierarchicalDataSource({
        transport: {
          read: {
            url: "/MenuManager/GetMenuTv",
            data: { id: dataItem.Id },
            dataType: "json",
          }
        },
        schema: {
          model: {
            id: "Id",
            children: "ChildMenus",
            hasChildren: "HasChildren"
          }
        }
      });

      parentMenuSelect = $("#ParentMenuId").kendoComboBox({
        placeholder: "Select Parent Menu",
        dataTextField: "Title",
        dataValueField: "Id",
        filter: "contains",
        autoBind: true,
        minLength: 3,
        dataSource: pmenuDataSrc
      }).data("kendoComboBox");
        $.each(data, function (key, value) {

            //alert(key + ":" + value);

            var ele = $('#' + (key == 'Id' ? 'hid' : key));
            if ($(ele).length > 0) {
                if (key == "MenuOrder") {
                    var ntxtBox = $("#MenuOrder").data("kendoNumericTextBox");
                    if (ntxtBox != null) {
                        ntxtBox.value(value);
                    }
                }
                if ((key == 'Id' ? 'hid' : key) == 'ParentMenuId' && parentMenuSelect != null) {
                    parentMenuSelect.value(value);
                }
                if ((key == 'Id' ? 'hid' : key) == 'ParentId') {
                    $('#ParentId').val(value);
                }
                if ((key == 'Id' ? 'hid' : key) == 'CategoryId') {
                    $('#CategoryId').val(value);
                }
                if ((key == 'Id' ? 'hid' : key) == 'ManufacturerId') {
                    $('#ManufacturerId').val(value);
                }

                if ((key == 'Id' ? 'hid' : key) == 'TopicId') {
                    $('#TopicId').val(value);
                }

                if ((key == 'Id' ? 'hid' : key) == 'SelectedCustomerRoleIds') {
                    $('#SelectedCustomerRoleIds').val(value);

                    var rolesIdsInput = $('#SelectedCustomerRoleIds').data("kendoMultiSelect");
                    rolesIdsInput.value(value);
                }



                if ((key == 'Id' ? 'hid' : key) == 'PictureUrl') {
                    $('#PictureUploader').find('img').attr('src', value);
                    $('#PictureUploader').find('span').css('display', '');
                }


                else if (ele[0].localName == 'input') {
                    if (($(ele)[0].type == 'text' || $(ele)[0].type == 'hidden')) {
                        ele.val(value);
                    }

                    else if ($(ele)[0].type == 'checkbox') {
                        ele.prop('checked', value);
                    }
                }

            }
            else {
                if ((key == 'Id' ? 'hid' : key) == 'Locales') {
                    for (var i = 0; i < value.length; i++) {
                        var lId = "#Locales_" + i + "__Title";
                        $(lId).val(value[i].Title);
                    }
                }
            }
        });
    }).error(function (err) {
        alert("an error occured during your request.")
    });
}

function onTreeDrop(e) {
    var srcItem = this.dataItem(e.sourceNode),
        tgtItem = this.dataItem(e.dropTarget);
    var uri = "/MenuManager/SaveMenu/" + srcItem.Id;
}

var selectedID;
var menuItems = new Array();
function SaveTreeChanges() {

    selectedID = '';

    var tv = $("#Menus-tree").data("kendoTreeView");

    selectedID = getRecursiveMenuItems(tv.dataSource.view(), null);

    var data = {};
    data.str = selectedID;
    var menuString = { Menus: menuItems };
    $.ajax({
        type: 'POST',
        url: '/MenuManager/UpdateMenus/',
        data: menuString,
        dataType: 'json',
        success: function (result) {
            tv.dataSource.read();
        },
        error: function (data, textStatus) {
            alert(textStatus);
        },
    });
}

function getRecursiveMenuItems(nodeview, parentId) {
    for (var i = 0; i < nodeview.length; i++) {
        var item = new MenuTv();
        item.Id = parseInt(nodeview[i].id);
        item.MenuName = nodeview[i].MenuName;
        item.ParentMenuId = parentId;
        menuItems.push(item);
        //nodeview[i].text; You can also access text here
        if (nodeview[i].hasChildren) {
            getRecursiveMenuItems(nodeview[i].children.view(), nodeview[i].id);
        }
    }

    return selectedID;
}

function getRecursiveNodeText(nodeview) {
    for (var i = 0; i < nodeview.length; i++) {
        selectedID += nodeview[i].id + ",";
        //nodeview[i].text; You can also access text here
        if (nodeview[i].hasChildren) {
            getRecursiveNodeText(nodeview[i].children.view());
        }
    }

    return selectedID;
}

function MenuTv() {
    this.Id = 0;
    this.MenuName = 0;
    this.ParentMenuId = null;
    this.HasChildren = false;
}