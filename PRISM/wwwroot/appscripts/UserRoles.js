$(document).ready(function () {
    var objCommon = new Common();

    var app = new Vue({
        el: '#apporg',
        data: {
            DataList: [],
            d: {},
            lookup: {},
            AddUpdate: "Add",
            RolesData: [],
            RoleId: 0,
            role: {
                Name:""
            }
        },
        methods: {
            Delete($event) {
                var param = {
                    Id: this.d.Id
                };

                objCommon.AjaxCall("/AppUsers/delete", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Deleted.", "success");
                    location.reload();
                }, $event.currentTarget);
            },
            Edit(data) {
                var json = JSON.parse(data);
                this.d = {};
                this.RoleId = Number(json.id);
                this.GetData(null, this.RoleId);

            },
            ShowLookup($event) {
                this.d = {};
                this.d.Id = 0;
            },
            Insert($event) {
                var et = this;
                var param = et.RolesData;
                objCommon.AjaxCall("/AppUsers/InsertModuleRoles", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Added.", "success");
                    et.AddUpdate = "Add";
                    location.reload();
                }, $event.currentTarget);

            },
            InsertRoles($event) {
                var et = this;
                var param = et.role;
                param.Description = param.Name;
                objCommon.AjaxCall("/AppUsers/insertRole", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Added.", "success");
                    location.reload();
                }, $event.currentTarget);

            },
            InsertLookup($event) {
                var et = this;
                var param = et.lookup;
                param.LookupType = "Employee";
                objCommon.AjaxCall("/AppUsers/insert", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Added.", "success");
                    //et.AddUpdate = "Add";
                    location.reload();
                }, $event.currentTarget);

            },
            GetData($event, roleId) {
                this.RoleId = Number(roleId);
                var dxthis = this;
                var data = {
                    RoleId: Number(roleId)
                }
                objCommon.AjaxCall("/AppUsers/GetModuleRoles", $.param(data), "GET", true, function (response) {
                    var jsTreeData = [];
                    $("#jstreeroles").jstree("destroy");
                    if (response != null) {
                        response.forEach(function (v) {
                            var childern = [];
                            if (v.ModuleId == 24 || v.ModuleId == 25
                                || v.ModuleId == 26 || v.ModuleId == 27) {

                                childern = [
                                    {
                                        text: "Read", id: v.ModuleId + "_" + 4,
                                        "state": {
                                            "selected": v.ActionRoles.IsRead
                                        }
                                    }];
                            }
                            else {
                                childern = [
                                    {
                                        text: "Insert",
                                        id: v.ModuleId + "_" + 1,
                                        "state": {
                                            "selected": v.ActionRoles.IsInsert

                                        }
                                    },
                                    {
                                        text: "Delete", id: v.ModuleId + "_" + 2,
                                        "state": {
                                            "selected": v.ActionRoles.IsDelete
                                        }
                                    },
                                    {
                                        text: "Edit", id: v.ModuleId + "_" + 3,
                                        "state": {
                                            "selected": v.ActionRoles.IsEdit
                                        }
                                    },
                                    {
                                        text: "Read", id: v.ModuleId + "_" + 4,
                                        "state": {
                                            "selected": v.ActionRoles.IsRead
                                        }
                                    }];
                            }
                            

                            jsTreeData.push({
                                text: v.ModuleName, id: v.ModuleId, children: childern
                            })
                        })
                    }

                    $('#jstreeroles').jstree({
                        "plugins": ["wholerow", "checkbox", "types"],
                        "core": {
                            "themes": {
                                "responsive": false
                            },
                            "data": jsTreeData
                        },
                        "types": {
                            "default": {
                                "icon": "fa fa-folder text-warning"
                            },
                            "file": {
                                "icon": "fa fa-file  text-warning"
                            }
                        },
                    });
                   
                    $('#jstreeroles').on("changed.jstree", function (e, data) {
                        const selectedNodeIds = data.selected;
                        dxthis.RolesData = [];
                       
                        var selectedNodesHierarchy = createHierarchy(data.selected);
                        selectedNodesHierarchy.forEach(function (v) {
                            
                            var childData = [];
                            childData[0] = false;
                            childData[1] = false;
                            childData[2] = false;
                            childData[3] = false;
                            
                          
                            v.children.forEach(function (c) {
                                childData[Number(c-1)] = true;
                            })
                            dxthis.RolesData.push({
                                ModuleId: Number(v.parent), RoleId: dxthis.RoleId, IsDelete: childData[1],
                                IsRead: childData[3], IsEdit: childData[2], IsInsert: childData[0]
                            })
                        })
                    });

                    function createHierarchy(inputArray) {
                        const hierarchyMap = new Map();

                        inputArray.forEach(item => {
                            const [parent, child] = item.split('_');

                            if (!hierarchyMap.has(parent)) {
                                hierarchyMap.set(parent, []);
                            }

                            if (child) {
                                hierarchyMap.get(parent).push(parseInt(child));
                            }
                        });

                        // Check if '1' is in the hierarchy, if not, add it with children
                        if (!hierarchyMap.has('1')) {
                            hierarchyMap.set('1', []);
                        }

                        const hierarchyArray = Array.from(hierarchyMap, ([parent, children]) => ({
                            parent: parseInt(parent),
                            children
                        }));

                        return hierarchyArray;
                    }

                }, $event == null ? null : $event.currentTarget);

            },
            Reset() {
                this.d = {};
                et.AddUpdate = "Add";
            }

        },
        created() {
            this.GetData(null, 1);
            //GetData(null);
            //var url = 'https://api.example.com/endpoint';
            //var postData = {
            //	key1: 'value1',
            //	key2: 'value2'
            //};

            //// Make the POST request using Axios
            //axios.post(url, postData)
            //	.then(function (response) {
            //		// Handle successful response
            //		console.log('Response:', response.data);
            //	})
            //	.catch(function (error) {
            //		// Handle error
            //		console.error('Error:', error);
            //	});

        },
        updated() {

        },
        mounted() {
        }
    })


    $(".nav-item").removeClass("active");
    $("#liUserManagement").addClass("active");
    $(".menu-item").removeClass("show");
    $(".menu-item").removeClass("here");
    $("#menuRoles").addClass("here");
    $("#menuRoles").addClass("show");
    $(".tab-pane").removeClass("active");
    $("#tabUserManagement").addClass("active");
    $(".tab-pane").removeClass("show");
    $("#tabUserManagement").addClass("show");

})