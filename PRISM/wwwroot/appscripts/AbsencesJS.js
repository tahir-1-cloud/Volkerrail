var objCommon = new Common();
$(document).ready(function () {


    var app = new Vue({
        el: '#apporgabs',
        data: {
            DataList: [],
            d: {},
            lookup: {},
            AddUpdate: "Add",
            EmployeeId: 0,
            AbsId: 0,
            SelecrtedName:""
        },
        methods: {
            Delete($event) {
                var et = this;
                var param = {
                    Id: this.AbsId
                };

                objCommon.AjaxCall("/Abs/delete", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Deleted.", "success");
                    et.GetData(null);
                }, $event.currentTarget);
            },
            GetAbsance(data) {
                var json = JSON.parse(data);
                this.SelecrtedName = json.firstName + " " + json.lastName;
                this.EmployeeId = json.id;
                $(".anchor").css("color", "#181C32");
                $("#anchor" + json.id).css("color", "#009ED6 !important");
                var dxthis = this;
                this.GetData(null);

              
            },
            Edit(data) {
                this.d = {};

                if (data.DateFromString != null && data.DateFromString != "") {
                    data.DateFromString = data.DateFromString.replaceAll("/", "-");

                    var parts = data.DateFromString.split("-");
                    var dated = new Date(parts[2], parts[1] - 1, parts[0]);
                    let minDate = new Date(dated);

                    $("#dateToString").datepicker("option", "minDate", minDate);

                }
                if (data.DateToString != null && data.DateToString != "") {
                    data.DateToString = data.DateToString.replaceAll("/", "-");
                }

                this.d = data;


            },
            ShowLookup($event) {
                this.d = {};
                this.d.Id = 0;
            },
            Insert($event) {
                if (!objCommon.Validate("#inputform"))
                    return;
                var et = this;
                var param = et.d;        
                param.EmployeeId = et.EmployeeId;           
                objCommon.AjaxCall("/Abs/insert", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Added.", "success");
                    //et.AddUpdate = "Add";
                    et.GetData(null);
                    et.d = {};
                }, $event.currentTarget);

            },
            InsertLookup($event) {
                var et = this;
                var param = et.lookup;
                param.LookupType = "Employee";
                objCommon.AjaxCall("/Lookups/insert", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Added.", "success");
                    //et.AddUpdate = "Add";
                    location.reload();
                }, $event.currentTarget);

            },
            GetData($event) {
                var dxthis = this;
                dxthis.d = {};
                var datadb = {
                    PageIndex: 1,
                    PageSize: 100,
                    SortColumn: "DateFrom",
                    SortOrder: "desc",
                    SearchText: "text",
                    EmployeeId:this.EmployeeId
                }
               
                objCommon.AjaxCall("/Abs/GetData", $.param(datadb), "GET", true, function (response) {


                    var columnNames = [
                        { dataField: 'DateFromString', caption: 'Date From', dataType: 'date' },
                        { dataField: 'DateToString', caption: 'Date To', dataType: 'date' },
                        { dataField: 'LeaveType', caption: 'Type', dataType: 'string' }, { dataField: 'Reason',caption:'Description', dataType: 'string' }];
                    const employeesStore = new DevExpress.data.ArrayStore({
                        key: 'Id',
                        data: response,
                    });

                    const dataGrid = $('#gridAbsance').dxDataGrid({

                        dataSource: employeesStore,
                        showBorders: true,
                        columns: columnNames,
                        loadPanel: {
                            enabled: true,
                        },
                        scrolling: {
                            columnRenderingMode: 'virtual',
                        },
                        columnsAutoWidth: true,
                        focusedRowEnabled: true,
                        paging: {
                            enabled: false,
                        },
                        editing: {
                            mode: 'cell',
                        },
                        
                        //columns: ListCol,
                        sorting: {
                            mode: "multiple"
                        },
                        allowColumnResizing: true,
                        onRowClick: function (e) {
                            //console.log(e.data);
                            dxthis.AbsId = e.data.Id;
                           dxthis.Edit(e.data);
                            //console.log(e.data);
                        },
                        rowAlternationEnabled: true,
                        onEditingStart() {
                            //console.log('EditingStart');
                        },

                        onRowRemoving: function (e) {

                        },
                        onContentReady(e) {
                            e.component.option('loadPanel.enabled', false);
                        },
                        onSelectionChanged(data) {
                            dataGrid.option('toolbar.items[1].options.disabled', !data.selectedRowsData.length);
                        },
                    }).dxDataGrid('instance');

                });


                setTimeout(function () {
                    $("#datefromString").datepicker({
                        dateFormat: 'dd-mm-yy',//check change
                        onSelect: function (date) {
                            dxthis.d.DateFromString = date;
                            dxthis.d.DateToString = date;
                            dxthis.$forceUpdate();

                            var parts = date.split("-");
                            var dated = new Date(parts[2], parts[1] - 1, parts[0]);
                            let minDate = new Date(dated);

                            $("#dateToString").datepicker("option", "minDate", minDate);

                        },
                    });
                    $("#dateToString").datepicker({
                        dateFormat: 'dd-mm-yy',//check change
                        onSelect: function (date) {
                            dxthis.d.DateToString = date;
                            dxthis.$forceUpdate();
                        },
                    });

                }, 200);
               
            },
            Reset() {
                this.d = {};
                et.AddUpdate = "Add";
            }

        },
        created() {
            this.EmployeeId = EmpId;
            this.GetData(null);
            $(".anchor").css("color", "#181C32");
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
    $("#liEmployees").addClass("active");
    $(".menu-item").removeClass("show");
    $(".menu-item").removeClass("here");
    $("#menuAbsances").addClass("here");
    $("#menuAbsances").addClass("show");
    $(".tab-pane").removeClass("active");
    $(".tab-pane").removeClass("show");
    $("#tabEmployees").addClass("active");
    $("#tabEmployees").addClass("show");

})