var objCommon = new Common();
$(document).ready(function () {


    var Files = [];
    var dataGrid = null;
    var app = new Vue({
        el: '#apporg',
        data: {
            DataList: [],
            HeaderCol: [],
            ShiftData: [],
            SelectedShiftData: [],
            d: {},
            dReal: {},
            q: {},
            log: {},
            RoutesList: [],
            VSTP: {
                ShiftId: 0,
                Vstpcontact: 0,
                OriginLoc: "",
                OriginName: "",
                OriginStanox: "",
                OriginTime: "",
                DestLoc: "",
                DestName: "",
                DestStanox: "",
                DestTime: "",
                HeadCode: "",
                NumberOfVehicles: "",
                Comments: ""
            },
            Paging: {
                PageIndex: 0
            },
            FirstRec: 0,
            LastRec: 0,
            TotalRecords: 0,
            search: {
            },
            ContactList: [],
            LogEntryData: [],
            EmployeeData: [],
            DriverData: [],
            FileColumns: [],
            ViewColumns: [],
            ImportData: [{
                FilesColName: [],
                DBColName: []
            }],
            ActionType: "",
            ShiftId: 0,
            Roster: {
                EmaployeeData: [],
                DepartmentData: [],
                RosterData: [],
                department: ''
            },
            Absance: {
                EmployeeId: 0,
                AbsId: 0,
                FromDate: "",
                Todate: "",
                LeaveType: "",
                Reason: ""
            },
            RosterShift: {
                StartDateTime: "",
                FinishDateTime: "",
                DriverName: "",
                ShiftLocation: "",
                Description: "",
                ShiftId: 0
            },
            contact: {

            },
            StandardFilters: [],
            SelectedDistData: [],
            ShiftRowData: {}
            ,
            comment: {
                ListArrangements: [],
                EngineeringSupport: "",
                CoursesAndOthers: ""
            },
            item: {

            },

            ContactTypeObject: []

        },

        methods: {


            Delete(id) {
                var param = {
                    Id: id
                };

                objCommon.AjaxCall("/Home/delete", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Deleted.", "success");
                    location.reload();
                });
            },
            Edit(data) {
                var dxthis = this;
                // var json = JSON.parse(data);
                this.d = {};
                this.dReal = data;
                var dataCopy = Object.assign({}, data);
                const dateObject = new Date(dataCopy.StartDateTime);
                const finishdateObject = new Date(dataCopy.FinishDateTime);
                if (!isNaN(dateObject)) {
                    dataCopy.StartDateTime = moment(dateObject).format("DD-MM-YYYY");
                    dataCopy.FinishDateTime = moment(finishdateObject).format("DD-MM-YYYY");
                    //    // Format the Date object to a string in "YYYY-MM-DD" format
                    //    //  data.StartDateTime = dateObject.toISOString().split('T')[0];

                }
                console.log("datacopy", dataCopy);
                this.d = dataCopy;


                this.d.WeekNo = this.d.WeekNo.slice(1);
                $("#floatingPlannedHours").val(this.d.PprePlannedHours);
                this.ShiftId = dataCopy.Id;


                objCommon.AjaxCall("/Home/GetDataById", $.param({ id: this.ShiftId }), "GET", true, function (response) {
                    //response.ContactTypeObject
                    //response.EmployeeObject
                    //response.MilesStoneObject
                    dxthis.ContactTypeObject = response.ContactTypeObject;
                    dxthis.RoutesList = response.RoutesList;
                    dxthis.LogEntryData = [];
                    if (response.MilesStoneObject != undefined && response.MilesStoneObject != null) {
                        response.MilesStoneObject.forEach(function (v, ind) {
                            dxthis.LogEntryData.push({
                                Id: v.Id, MileStoneActivity: v.MileStoneEntryDetail, LogType: v.LogEntry, Planned: v.Planned, Actuall: v.Actuall, Comments: v.Comments
                            });

                            //  dxthis.GridModelGrids(dxthis.LogEntryData, "GridMileStone");
                        });
                    }

                    dxthis.GridModelGrids(dxthis.LogEntryData, "GridMileStone");



                    dxthis.DriverData = response.EmployeeObject == null ? [] : response.EmployeeObject.filter(x => x.EmployeeType == "GridDriver");
                    dxthis.GridModelGrids(response.EmployeeObject == null ? [] : response.EmployeeObject.filter(x => x.EmployeeType == "GridCrewManager"), "GridCrewManager", "update");
                    dxthis.GridModelGrids(response.EmployeeObject == null ? [] : response.EmployeeObject.filter(x => x.EmployeeType == "GridCrewOperator"), "GridCrewOperator", "update");
                    // dxthis.GridModelGrids(response.MilesStoneObject, "GridMileStone", "update");
                    dxthis.GridModelGrids(response.EmployeeObject == null ? [] : response.EmployeeObject.filter(x => x.EmployeeType == "GridConductor"), "GridConductor", "update");
                    dxthis.GridModelGrids(response.EmployeeObject == null ? [] : response.EmployeeObject.filter(x => x.EmployeeType == "GridDriver"), "GridDriver", "update");
                    dxthis.GridModelGrids(response.EmployeeObject == null ? [] : response.EmployeeObject.filter(x => x.EmployeeType == "GridAssesor"), "GridAssesor", "update");
                    dxthis.GridModelGrids(response.ContactTypeObject == null ? [] : response.ContactTypeObject, "GridCombine", "update");

                    $("#ShiftDetail").modal("show");

                    $("#shiftdetaildate").datepicker({
                        setDate: dateObject,
                        dateFormat: 'dd-mm-yy',//check change
                        onSelect: function (dateText, inst) {
                            dxthis.d.StartDateTime = dateText;
                            // Convert the selected date to a Date object
                            var parts = dateText.split("-");
                            var date = new Date(parts[2], parts[1] - 1, parts[0]);

                            // Get the day of the week (0 = Sunday, 1 = Monday, ..., 6 = Saturday)
                            var dayOfWeek = date.getDay()
                            //var dayName = getDayName(selectedDate);
                            var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

                            dxthis.d.PpreDay = days[dayOfWeek];
                            var DateTimeMix = dateText + " " + $("#floatingStartTime").val();

                            objCommon.AjaxCall("/Home/GetShiftDayWeek", $.param({ DateTimeMix: DateTimeMix }), "GET", true, function (response) {
                                if (response != null) {
                                    dxthis.d.ShiftNo = "";
                                    dxthis.d.WeekNo = "";
                                    dxthis.d.ShiftNo = response.ShiftNumber;

                                    if (response.WeekNumber != undefined && response.WeekNumber != "" && response.WeekNumber != null) {
                                        dxthis.d.WeekNo = response.WeekNumber.split(' ')[2];
                                    }
                                    dxthis.$forceUpdate();
                                }
                            })


                            var dated = new Date(parts[2], parts[1] - 1, parts[0]);
                            let minDate = new Date(dated);

                            $("#shiftfinishdate").datepicker("option", "minDate", minDate);
                        }
                    });


                    var partst = dataCopy.StartDateTime.split("-");

                    var datedt = new Date(partst[2], partst[1] - 1, partst[0]);
                    let minDatet = new Date(datedt);
                    $("#shiftfinishdate").datepicker({
                        setDate: finishdateObject,
                        dateFormat: 'dd-mm-yy',//check change
                        minDate: minDatet,
                        onSelect: function (dateText, inst) {
                            dxthis.d.FinishDateTime = dateText;
                            dxthis.$forceUpdate();
                        }
                    });
                    function getDayName(date) {
                        var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
                        return days[date.getDay()];
                    }

                });

            },
            ShowLookup($event) {
                this.d = {};
                this.d.Id = 0;
                $("#lookupmodel").modal("show");
            },

            Insert($event) {
                var dxthis = this;
                //var dataSend = {};
                //for (key in this.d) {
                //    var ekKey = key;
                //    key = key.charAt(0).toUpperCase() + key.slice(1);

                //    dataSend[key] = this.d[ekKey];

                //}
                // console.log(dataSend);
                //var param = this.d;
                this.d.WorkCompleted = this.d.WorkCompleted;
                this.d.StartDateTime = $("#shiftdetaildate").val();
                this.d.StartTime = $("#floatingStartTime").val();
                this.d.FinishTime = $("#floatingFinishTime").val();
                this.d.PprePlannedHours = $("#floatingPlannedHours").val();

                if (this.d.StartDateTime == "" || this.d.StartDateTime == null || this.d.StartDateTime == undefined) {
                    objCommon.ShowMessage("Please select start date.", "error");
                    return;
                }
                if (this.d.StartTime == "" || this.d.StartTime == null || this.d.StartTime == undefined) {
                    objCommon.ShowMessage("Please select start time.", "error");
                    return;
                }
                if (this.d.FinishTime == "" || this.d.FinishTime == null || this.d.FinishTime == undefined) {
                    objCommon.ShowMessage("Please select finish time.", "error");
                    return;
                }
                if (this.d.MachineNum == "" || this.d.MachineNum == null || this.d.MachineNum == undefined) {
                    objCommon.ShowMessage("Please select Machine Num.", "error");
                    return;
                }
                if (this.d.WorksiteDetails == "" || this.d.WorksiteDetails == null || this.d.WorksiteDetails == undefined) {
                    objCommon.ShowMessage("Please select Site.", "error");
                    return;
                }
                if (this.d.YardIn == "" || this.d.YardIn == null || this.d.YardIn == undefined) {
                    objCommon.ShowMessage("Please select Yar In", "error");
                    return;
                }
                if (this.d.YardOut == "" || this.d.YardOut == null || this.d.YardOut == undefined) {
                    objCommon.ShowMessage("Please select Yard Out.", "error");
                    return;
                }
                this.d.ShiftNo = this.d.ShiftNo + "";
                console.log(this.d);
                // return;
                objCommon.AjaxCall("/Home/insert", JSON.stringify(this.d), "POST", true, function (response) {
                    if (response.Value == "success") {

                        objCommon.ShowMessage("Added.", "success");

                        // $("#lookupmodel").modal("hide");
                        dxthis.GetData(null, dxthis.search.PageIndex, dxthis.d.Id)
                        dxthis.GridModelGrids([], "GridCrewManager", "update");
                        dxthis.GridModelGrids([], "GridCrewOperator", "update");
                        dxthis.GridModelGrids([], "GridMileStone", "update");
                        dxthis.GridModelGrids([], "GridConductor", "update");
                        dxthis.GridModelGrids([], "GridDriver", "update");
                        dxthis.GridModelGrids([], "GridAssesor", "update");
                        dxthis.GridModelGrids([], "GridCombine", "update");
                        dxthis.ShiftId = dxthis.d.Id;
                        // $("#ShiftDetail").modal("hide");
                        if (dxthis.ActionType == "Update")
                            location.reload();
                    }
                    else {
                        objCommon.ShowMessage(response.Value, "error");
                    }
                }, $event.currentTarget);

            },
            GetData($event, num, keyn) {
                var dxthis = this;

                // $("#DriverMedication").modal("show");
                dxthis.StandardFilters = [];
                dxthis.StandardFilters.push({ FieldValue: 'OTM', Caption: 'Omit MUF Recieved', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'OTPM', Caption: 'Omit Log Printed', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'OTT', Caption: 'Omit Manager AH', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ACT', Caption: 'Omit Workshops', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ShiftBlank', Caption: 'Omit Blanks', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ShiftCaped', Caption: 'Omit Caped', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ShiftCancelled', Caption: 'Omit Cancelled', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'Owner', Caption: 'OmitHire-in Machine', Checked: false });
                //dxthis.StandardFilters.push({ FieldValue: '', Caption: '', Checked: false });
                var pageNum = localStorage.getItem("PageIndex");
                if (pageNum != undefined && pageNum != "" && pageNum != null) {
                    num = pageNum;
                }
                else {
                    num = 1;
                }


                dxthis.search.PageIndex = num;
                dxthis.search.PageSize = 100;
                if (dxthis.search.SortColumn == undefined) {
                    dxthis.search.SortColumn = "Id";
                }
                if (dxthis.search.SortColumn == undefined) {
                    // Set default sorting order
                    dxthis.search.SortOrder = "desc";
                }



                //if (dxthis.search.sortOrder == undefined) {
                //    dxthis.search.sortOrder = "desc";
                //}
                dxthis.search.SearchText = "";

                objCommon.AjaxCall("/Home/GetData", $.param(dxthis.search), "GET", true, function (response) {

                    dxthis.comment = response.WeeklyComments;
                    if (response.WeeklyComments.ListArrangements == null) {
                        dxthis.comment.ListArrangements = [];
                    }

                    dxthis.EmployeeData = response.EmployeeData;
                    if (dxthis.EmployeeData != null && dxthis.EmployeeData != undefined) {
                        dxthis.EmployeeData.forEach(function (v) {
                            v.FullName = (v.FirstName == null ? "" : v.FirstName) + ' ' + (v.LastName == null ? "" : v.LastName);
                        })
                    }
                    dxthis.ContactList = response.ContactData;
                    dxthis.ShiftData = response.ShiftData;
                    $("#spanFromWeek").html($("#ddlFromWeek option:selected").text());
                    $("#spanToWeek").html($("#ddlToWeek option:selected").text() != "" ? "-" + $("#ddlToWeek option:selected").text() : "");

                    response.ColumnNames.forEach(function (key) {
                        if (key != "Id") {
                            if (!key.includes("Ppre"))
                                dxthis.HeaderCol.push(key);
                        }
                    })

                    var lpShiftStatus = LookupData.filter(x => x.LookupType == 'ShiftStatus');
                    var lpACT = LookupData.filter(x => x.LookupType == 'ACT');
                    var GridCol = [
                        { dataField: 'PprePlannedHours', caption: 'R', allowEditing: true, allowSorting: false },
                        { dataField: 'Ptonumber', caption: 'PTO No.', allowEditing: true, allowSorting: true },
                        // { dataField: 'Ptonumber', caption: 'PTO No.', allowEditing: true, allowSorting: true, validationRules: [{ type: 'required' }] },
                        { dataField: 'Id', caption: 'MUF No.', allowEditing: false, allowSorting: true },
                        { dataField: 'MachineNum', caption: 'M/C No.', allowEditing: false, allowSorting: true },
                        { dataField: 'MachineType', caption: 'Type', allowEditing: false, allowSorting: false },
                        //{ dataField: 'MachineMgr', caption: 'Dept', allowEditing: false, allowSorting: false },
                        {
                            dataField: 'MachineMgr', caption: 'Dept', allowEditing: false, allowSorting: false,
                            calculateCellValue(data) {
                                return data['MachineMgr'] === 'OTM' ? 'OTM' : null; // Set value to OTM if it's OTM, otherwise set to null
                            }
                        },
                        {
                            dataField: 'PpreLogNumber', caption: 'Log', allowEditing: true, allowSorting: false
                        },
                        {
                            dataField: 'ShiftNo', caption: 'Shift', allowEditing: false, allowSorting: true

                        },
                        { dataField: 'PpreDay', caption: 'Day', allowEditing: true, allowSorting: false },
                        {
                            dataField: 'WeekNo', caption: 'Week',
                            allowEditing: true, allowSorting: true
                        },
                        {
                            dataField: 'PpreAct', caption: 'ACT',
                            allowEditing: true,
                            lookup: {
                                dataSource: lpACT == undefined ? [] : lpACT,
                                displayExpr: "LookupName",
                                valueExpr: 'LookupName',
                                searchEnabled: true,
                            }, allowSorting: true

                        },
                        {

                            dataField: 'StartDateTime', caption: 'Start', allowEditing: false, dataType: 'date', width: '200px', allowSorting: true
                            , format: "dd-MM-yyyy HH:mm"

                        },
                        {
                            dataField: 'FinishDateTime', caption: 'Finish', allowEditing: false, dataType: 'date', width: '200px', allowSorting: false
                            , format: "dd-MM-yyyy HH:mm"
                        },
                        { dataField: 'PpreDra', caption: 'DRA', allowEditing: true, allowSorting: false },
                        { dataField: 'WorksiteDetails', caption: 'Location', allowEditing: true, allowSorting: false, width: '240px' },
                        { dataField: 'Ppsreference', caption: 'Worksite Ref', allowEditing: true, allowSorting: false },
                        {
                            dataField: 'Shift', caption: 'Shift Status', allowEditing: true, allowSorting: false,
                            lookup: {
                                dataSource: lpShiftStatus == undefined ? [] : lpShiftStatus,
                                displayExpr: "LookupName",
                                valueExpr: 'LookupName',
                                searchEnabled: true,
                            },
                            calculateCellValue(data) {

                                if (data['Shift'] == undefined || data['Shift'] == null || data['Shift'] == "" || data['Shift'] == " ") {

                                    return "Live";
                                }
                                else
                                    return data['Shift'];

                            }
                        },
                        { dataField: 'PprePlfield', caption: 'Remark', allowEditing: true, allowSorting: false },
                        // { dataField: 'Remarks', caption: 'Remarks', allowEditing: true, allowSorting: false },
                        { dataField: 'Customer', caption: 'Company', allowEditing: true, allowSorting: false },
                        { dataField: 'Contractor', caption: 'Division', allowEditing: false, allowSorting: false },
                        { dataField: 'Route', caption: 'Zone', allowEditing: true, allowSorting: false },
                        { dataField: 'WorksiteElr', caption: 'ELR', allowEditing: true, allowSorting: false },
                        { dataField: 'PpreRTNo', caption: 'RT No.', allowEditing: true, allowSorting: false },
                        { dataField: 'PriorityStatusShift', caption: 'TOC', allowEditing: true, allowSorting: false },
                        { dataField: 'PPreContractor', caption: 'Cond', allowEditing: false, allowSorting: false },
                        { dataField: 'OutShortCode', caption: 'From', allowEditing: false, allowSorting: false },
                        { dataField: 'InShortCode', caption: 'To', allowEditing: false, allowSorting: false },
                        { dataField: 'PpreInternalComments', caption: 'Int Comm', allowEditing: true, allowSorting: false },
                        { dataField: 'WonNumber', caption: 'WON', allowEditing: true, allowSorting: true },
                        { dataField: 'PpreOTML', caption: 'OTML', allowEditing: false, allowSorting: false },
                        { dataField: 'PpreOperator', caption: 'Operators', allowEditing: false, allowSorting: false, width: '350px' },
                        { dataField: 'PpreES', caption: 'ES', allowEditing: false, allowSorting: false },
                        { dataField: 'PpreTQS', caption: 'TQS', allowEditing: false, allowSorting: false },
                        { dataField: 'PpreTQSPhone', caption: 'TQS Phone', allowEditing: false, allowSorting: false },
                        { dataField: 'PprePICOPPhone', caption: 'PICOP Phone', allowEditing: false, allowSorting: false },
                        { dataField: 'PpreMCO', caption: 'MCO', allowEditing: false, allowSorting: false },
                        { dataField: 'PpreAPM', caption: 'APM', allowEditing: false, allowSorting: false },
                        { dataField: 'PpreAPMPhone', caption: 'APM Phone', allowEditing: false, allowSorting: false },
                        { dataField: 'ShiftTimeDetail', caption: 'Shift Time Detail', allowEditing: false },
                    ];

                    console.log("GridColvalues", GridCol)
                    var RealColumns = [];
                    var IsIdExist = false;
                    if (dxthis.ViewColumns != null && dxthis.ViewColumns != undefined) {
                        dxthis.ViewColumns.forEach(function (v) {
                            var col = GridCol.filter(x => x.dataField == v)[0];
                            if (col != undefined)
                                RealColumns.push(col);

                            if (v == "Id") {
                                IsIdExist = true;
                            }
                        })
                    }
                    if (!IsIdExist) {
                        var col = GridCol.filter(x => x.dataField == "Id")[0];
                        col.visible = false;
                        RealColumns.push(col);
                    }

                    dxthis.ImportData.DBColName = dxthis.HeaderCol;

                    var focuskey = 0;
                    if (response.ShiftData == null || response.ShiftData == undefined || response.ShiftData.length <= 0) {
                        focuskey = 0;
                        localStorage.setItem("PageIndex", 1);
                    }
                    else
                        focuskey = response.ShiftData[0].Id;


                    var columnDet = RealColumns.filter(x => x.dataField == (dxthis.search.SortColumn == "StartDate" ? "StartDateTime" : dxthis.search.SortColumn))[0];
                    console.log(columnDet, dxthis.search.SortColumn, dxthis.search.SortOrder);
                    columnDet.sortOrder = dxthis.search.SortOrder;

                    const employeesStore = new DevExpress.data.ArrayStore({
                        key: 'Id',
                        data: response.ShiftData,
                    });
                    if (SecurityModel.IsRead == "True") {
                        if (dataGrid != null) {
                            dataGrid.dispose();
                            $('#gridContainer').empty();
                        }

                        dataGrid = $('#gridContainer').dxDataGrid({

                            dataSource: employeesStore,
                            columns: RealColumns,
                            showBorders: true,
                            height: 600,
                            selection: {
                                mode: 'multiple',
                            },
                            loadPanel: {
                                enabled: true,
                            },
                            columnWidth: 100,
                            scrolling: {
                                columnRenderingMode: 'virtual',
                            },
                            columnsAutoWidth: true,
                            focusedRowEnabled: true,
                            focusedRowKey: keyn == 0 ? focuskey : keyn,
                            paging: {
                                enabled: false,
                            },
                            editing: {
                                allowUpdating: SecurityModel.IsEdit === "True" && SecurityModel.RoleId != 11,
                                // allowUpdating: SecurityModel.IsEdit == "True" ? true : false,
                                mode: 'row',
                            },
                            columnFixing: {
                                enabled: true // Enable column fixing
                            },
                            export: {
                                enabled: true,
                                allowExportSelectedData: true,
                            },
                            onExporting(e) {
                                const workbook = new ExcelJS.Workbook();
                                const worksheet = workbook.addWorksheet('LNEDetail');

                                DevExpress.excelExporter.exportDataGrid({
                                    component: e.component,
                                    worksheet,
                                    autoFilterEnabled: true,
                                }).then(() => {
                                    workbook.xlsx.writeBuffer().then((buffer) => {
                                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'LNEDetail.xlsx');
                                    });
                                });
                            },
                            sorting: {
                                mode: "multiple"
                            },
                            allowColumnResizing: true,
                            onRowDblClick: function (e) {
                                dxthis.ActionType = "Update";
                                dxthis.Edit(e.data);
                            },
                            rowAlternationEnabled: true,
                            columnChooser: {
                                enabled: true,
                                mode: "select",
                                position: {
                                    my: 'right top',
                                    at: 'right bottom',
                                    of: '.dx-datagrid-column-chooser-button',
                                },
                                search: {
                                    enabled: true,
                                    editorOptions: { placeholder: 'Search column' },
                                },
                                selection: {
                                    recursive: true,
                                    selectByClick: true,
                                    allowSelectAll: true,
                                },
                            },
                            onEditingStart() {
                                //console.log('EditingStart');
                            },
                            onRowUpdating: function (e) {
                                console.log(e.oldData);
                                //var columnName = "";
                                dxthis.d = e.oldData;
                                for (key in e.newData) {

                                    dxthis.d[key] = e.newData[key];
                                }
                                dxthis.d.Id = e.key;
                                //  return;

                                objCommon.AjaxCall("/Home/insert", JSON.stringify(dxthis.d), "POST", true, function (response) {
                                    objCommon.ShowMessage("Added.", "success");

                                });
                            },
                            onRowRemoving: function (e) {

                            },
                            onContentReady(e) {
                                e.component.option('loadPanel.enabled', false);
                            },
                            onCellPrepared: function (e) {
                                if (e.rowType === "data") {
                                    if (e.data.Shift == "Provisional") {
                                        e.cellElement.css({ "color": "#D648D7" });

                                    }
                                    else if (e.data.Shift == "Cancelled" || e.data.Shift == "Caped") {
                                        e.cellElement.css({ "color": "red" });

                                    }
                                }
                            },
                            onSelectionChanged: function (e) {
                                // console.log(e, "abid");
                                dxthis.SelectedShiftData = e.selectedRowKeys;
                                dxthis.ShiftRowData = e.selectedRowsData[0];
                            },
                            onToolbarPreparing: function (e) {

                                e.toolbarOptions.items.unshift({
                                    location: "after",
                                    widget: "dxButton",
                                    options: {
                                        icon: "download",
                                        hint: "View Roster",
                                        onClick: function () {
                                            $("#btnoffcanvas").trigger("click");
                                            $('#overlay').toggle();
                                            $('html, body').toggleClass('noscroll');
                                        }
                                    }
                                }, {
                                    location: "after",
                                    widget: "dxButton",
                                    options: {
                                        icon: "add",
                                        hint: "Add Shift",
                                        onClick: function () {
                                            this.d.Shift = 'Live';
                                            $("#ddlShiftStatus").val('Live');
                                            dxthis.d = {};
                                            dxthis.ActionType = "Insert";
                                            dxthis.GridModelGrids([], "GridCrewManager", "new");
                                            dxthis.GridModelGrids([], "GridCrewOperator", "new");
                                            dxthis.GridModelGrids([], "GridMileStone", "new");
                                            dxthis.GridModelGrids([], "GridConductor", "new");
                                            dxthis.GridModelGrids([], "GridDriver", "new");
                                            dxthis.GridModelGrids([], "GridAssesor", "new");
                                            dxthis.GridModelGrids([], "GridCombine", "new");
                                            if (SecurityModel.IsInsert == "True") {
                                                objCommon.AjaxCall("/Home/GetLatestMUFNo", $.param({}), "GET", true, function (res) {
                                                    dxthis.d.Id = res;
                                                    dxthis.$forceUpdate();

                                                    $("#shiftdetaildate").datepicker({
                                                        dateFormat: 'dd-mm-yy',//check change
                                                        onSelect: function (date) {
                                                            dxthis.d.StartDateTime = date;
                                                            var parts = date.split("-");
                                                            var date = new Date(parts[2], parts[1] - 1, parts[0]);

                                                            // Get the day of the week (0 = Sunday, 1 = Monday, ..., 6 = Saturday)
                                                            var dayOfWeek = date.getDay()
                                                            //var dayName = getDayName(selectedDate);
                                                            var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

                                                            dxthis.d.PpreDay = days[dayOfWeek];

                                                            var DateTimeMix = dxthis.d.StartDateTime + " " + $("#floatingStartTime").val();
                                                            console.log(DateTimeMix);
                                                            objCommon.AjaxCall("/Home/GetShiftDayWeek", $.param({ DateTimeMix: DateTimeMix }), "GET", true, function (response) {
                                                                if (response != null) {
                                                                    dxthis.d.ShiftNo = response.ShiftNumber;
                                                                    if (response.WeekNumber != undefined && response.WeekNumber != "" && response.WeekNumber != null)
                                                                        dxthis.d.WeekNo = response.WeekNumber.split(' ')[2];

                                                                    dxthis.$forceUpdate();
                                                                }
                                                            })
                                                            var dated = new Date(parts[2], parts[1] - 1, parts[0]);
                                                            let minDate = new Date(dated);

                                                            $("#shiftfinishdate").datepicker("option", "minDate", minDate);

                                                        },
                                                    });

                                                    $("#shiftfinishdate").datepicker({
                                                        dateFormat: 'dd-mm-yy',//check change
                                                        onSelect: function (dateText, inst) {
                                                            dxthis.d.FinishDateTime = dateText;
                                                            dxthis.$forceUpdate();
                                                        }
                                                    });



                                                    $("#ShiftDetail").modal("show");
                                                });
                                            }
                                        }
                                    }
                                },

                                    {
                                        location: "after",
                                        widget: "dxButton",
                                        options: {
                                            icon: "import",
                                            hint: "Upload Shifts",
                                            onClick: function () {
                                                $("#FileUploadeModel").modal("show");

                                            }
                                        }
                                    },
                                    {
                                        location: "after",
                                        widget: "dxButton",
                                        options: {
                                            icon: "edit",
                                            hint: "Change Shift Status",
                                            onClick: function () {
                                                if (dxthis.SelectedShiftData.length > 0) {

                                                    $("#ShiftStatusModel").modal("show");
                                                    console.log(dxthis.ShiftRowData);
                                                    $("#ddlShiftStatus2").val(dxthis.ShiftRowData.Shift);
                                                }
                                                else {
                                                    objCommon.ShowMessage("please select rows first.", "error");
                                                }

                                            }
                                        }
                                    }
                                    ,
                                    {
                                        location: "after",
                                        widget: "dxButton",
                                        options: {
                                            icon: "copy",
                                            hint: "Duplicate Shifts",
                                            onClick: function () {





                                                if (dxthis.SelectedShiftData.length > 0) {
                                                    var et = this;

                                                    var IsHoursAdded = false;

                                                    Swal.fire({
                                                        title: "Do you want to add 24 hours?",
                                                        text: "You won't be able to revert this!",
                                                        icon: "warning",
                                                        showCancelButton: true,
                                                        confirmButtonColor: "#3085d6",
                                                        cancelButtonColor: "#d33",
                                                        confirmButtonText: "Yes",
                                                        cancelButtonText: "No"
                                                    }).then((result) => {
                                                        if (result.isConfirmed) {
                                                            IsHoursAdded = true;
                                                        }
                                                        else {

                                                        }


                                                        var param = {
                                                            Ids: dxthis.SelectedShiftData,
                                                            IsHoursAdded: IsHoursAdded
                                                        }
                                                        objCommon.AjaxCall("/Home/DuplicateRecords", JSON.stringify(param), "POST", true, function (response) {
                                                            if (response != null) {

                                                                objCommon.ShowMessage("Added.", "success");
                                                                dxthis.GetData(null, dxthis.search.PageIndex, response.Id);
                                                            }
                                                            else {
                                                                objCommon.ShowMessage(response, "error");
                                                            }



                                                        });

                                                    });


                                                }
                                                else {
                                                    objCommon.ShowMessage("please select rows first.", "error");
                                                }

                                            }
                                        }
                                    }
                                    ,
                                    {
                                        location: "after",
                                        widget: "dxButton",
                                        options: {
                                            icon: "email",
                                            hint: "PLANT AMENDMENT CHANGE",
                                            onClick: function () {
                                                if (dxthis.SelectedShiftData.length > 0 && dxthis.SelectedShiftData.length == 1) {

                                                    console.log(response.DistributionList);

                                                    $("#DistributionListPopup").modal("show");
                                                    if (response.DistributionList != null && response.DistributionList != "") {

                                                        response.DistributionList.forEach(function (v, ind) {
                                                            v.Id = (ind + 1);
                                                        })

                                                        const DistributionStore = new DevExpress.data.ArrayStore({
                                                            key: 'Id',
                                                            data: response.DistributionList,
                                                        });

                                                        var GridColol = [
                                                            { dataField: 'Id', caption: 'Id', visible: false },
                                                            { dataField: 'TypeId', caption: 'TypeId', visible: false },
                                                            { dataField: 'TypeName', caption: 'Distribution List', visible: true },
                                                            { dataField: 'EmailAddress', caption: 'Emails', visible: true },
                                                            { dataField: 'ActiveStatus', caption: 'Status', visible: false }
                                                        ];

                                                        var disGrid = $('#gridDistributionList').dxDataGrid({

                                                            dataSource: DistributionStore,
                                                            columns: GridColol,
                                                            showBorders: true,
                                                            selection: {
                                                                mode: 'multiple',
                                                            },
                                                            columnsAutoWidth: true,
                                                            paging: {
                                                                enabled: true,
                                                            },
                                                            sorting: {
                                                                mode: "multiple"
                                                            },
                                                            rowAlternationEnabled: true,
                                                            onSelectionChanged: function (e) {
                                                                dxthis.SelectedDistData = e.selectedRowsData;

                                                            },
                                                        }).dxDataGrid('instance');



                                                    }
                                                    //var emailAddresses = "user1@example.com,user2@example.com"; // Add your email addresses separated by commas
                                                    //var subject = "Subject of the email";
                                                    //var body = "Body of the email";

                                                    //var mailtoLink = "mailto:" + emailAddresses +
                                                    //    "?subject=" + encodeURIComponent(subject) +
                                                    //    "&body=" + encodeURIComponent(body);

                                                }
                                                else {
                                                    objCommon.ShowMessage("please select 1 row.", "error");
                                                }

                                            }
                                        }
                                    }
                                );
                            },
                            onCellClick: function (e) {
                                if (e.rowType == "header") {
                                    if (e.column.name == "Ptonumber" || e.column.name == "Id" || e.column.name == "MachineNum"
                                        || e.column.name == "ShiftNo" || e.column.name == "WeekNo" || e.column.name == "WonNumber"
                                        || e.column.name == "StartDateTime" || e.column.name == "PpreAct") {
                                        KTApp.showPageLoading();
                                        dxthis.search.SortColumn = e.column.name == "StartDateTime" ? "StartDate" : e.column.name;
                                        //if (dxthis.search.SortOrder == "desc") {
                                        //    dxthis.search.SortOrder = "asc";
                                        //}
                                        //else {
                                        //    dxthis.search.SortOrder = "desc";
                                        //}
                                        dxthis.search.SortOrder = (dxthis.search.SortOrder === "desc") ? "" : "desc";

                                        //dxthis.search.SortOrder = (dxthis.search.SortOrder === "asc") ? "desc" : "asc";
                                        dxthis.$forceUpdate();
                                        dxthis.GetData(null, 1, 0);
                                    }
                                }
                            }
                        }).dxDataGrid('instance');

                    }

                    //d dataGrid.getView("gridContainer").setFocusedRowIndex(rowIndex);

                    function getGroupCount(groupField) {
                        return DevExpress.data.query(response.ShiftData)
                            .groupBy(groupField)
                            .toArray().length;

                    }


                    var importYes = localStorage.getItem("ImportData");
                    if (importYes == "Yes") {
                        localStorage.removeItem("ImportData");
                        $("#FileUploadeModel").modal("show");
                    }

                    //let uri = window.location.search.substring(1);
                    //let params = new URLSearchParams(uri);
                    //if (params.get("type") == "ByMenu") {
                    //    $("#FileUploadeModel").modal("show");
                    //}
                    if (response.ShiftData.length > 0) {
                        dxthis.FirstRec = response.ShiftData[0].Start;
                        dxthis.LastRec = response.ShiftData[0].End;
                        dxthis.TotalRecords = response.ShiftData[0].TotalRecords;
                        // if (dxthis.search.PageIndex == 1) {
                        $("#divPaginationAll").empty();
                        $("#divPaginationAll").pagination({
                            items: response.ShiftData[0].TotalRecords,
                            itemsOnPage: dxthis.search.PageSize,
                            currentPage: dxthis.search.PageIndex,
                            cssStyle: 'light-theme',
                            onPageClick: function (pageNum) {
                                localStorage.setItem("PageIndex", pageNum);
                                dxthis.GetData($event, pageNum, 0);
                            }
                        });
                        //   }

                    }
                    else {
                        dxthis.FirstRec = 0;
                        dxthis.LastRec = 0;
                        dxthis.TotalRecords = 0;
                        if (dxthis.search.PageIndex == 1) {
                            $("#divPaginationAll").empty();
                        }
                    }

                    KTApp.hidePageLoading();
                }, $event == null ? null : $event.currentTarget);

            },

            GridModelGrids(data, gridid, datatype) {
                var dxthis = this;
                var bigData = [];
                var FinalCol = [];
                var creatManager = this.EmployeeData;//.filter(x => x.employeeTypeId == 5);
                var OperatorData = this.EmployeeData;//.filter(x => x.employeeTypeId == 5);
                var ConductorData = this.EmployeeData;//.filter(x => x.employeeTypeId == 5);
                // var DriverData = this.EmployeeData;//.filter(x => x.employeeTypeId == 5);
                var AssessorData = this.EmployeeData;//.filter(x => x.employeeTypeId == 5);
                var AssessorData = this.EmployeeData;//.filter(x => x.employeeTypeId == 5);


                var ContactType = [{ Id: "PICOP", FullName: "PICOP" }, { Id: "ES", FullName: "ES" }, { Id: "TQS", FullName: "TQS" }, { Id: "Site Contacts", FullName: "Site Contacts" }]

                var ColumnList = [{ dataField: 'MileStoneActivity', caption: "Milestone Activity", dataType: 'string' },
                { dataField: 'Planned', dataType: 'string', width: "10%" },
                { dataField: 'Actuall', caption: 'Actual', dataType: 'string', width: "10%" },
                { dataField: 'Comments', dataType: 'string' }];


                var ColumnGridCrewManager = [


                    { dataField: 'Id', visible: false }, {
                        dataField: "EmployeeId",
                        caption: "Crew",
                        setCellValue(rowData, value) {

                            var singObj = dxthis.EmployeeData.filter(x => x.Id == value)[0];
                            rowData.EmployeeId = value;
                            rowData.JobTitle = singObj.JobTitle;
                            rowData.ContactNumber = singObj.ContactNumber;

                        },
                        lookup: {
                            dataSource: creatManager,
                            displayExpr: "FullName",
                            valueExpr: 'Id',
                            searchEnabled: true,
                        },

                    }, { dataField: 'JobTitle', dataType: 'string' }, {
                        dataField: 'ContactNumber', dataType: 'string'
                    }];

                var ColumnGridCrewOperator = [{ dataField: 'Id', visible: false }, {
                    dataField: "EmployeeId",
                    caption: "Crew",
                    setCellValue(rowData, value) {
                        var singObj = dxthis.EmployeeData.filter(x => x.Id == value)[0];
                        rowData.EmployeeId = value;
                        rowData.JobTitle = singObj.JobTitle;
                        rowData.ContactNumber = singObj.ContactNumber;

                    },
                    lookup: {
                        dataSource: creatManager,
                        displayExpr: "FullName",
                        valueExpr: 'Id',
                        searchEnabled: true,
                    },

                }, { dataField: 'JobTitle', dataType: 'string' }, {
                    dataField: 'ContactNumber', dataType: 'string'
                }];

                var ColumnGridConductor = [{ dataField: 'Id', visible: false }, {
                    dataField: "EmployeeId",
                    caption: "Conductor",
                    setCellValue(rowData, value) {
                        var singObj = dxthis.EmployeeData.filter(x => x.Id == value)[0];
                        rowData.EmployeeId = value;
                        //rowData.JobTitle = singObj.JobTitle;
                        rowData.ContactNumber = singObj.ContactNumber

                    },
                    lookup: {
                        dataSource: creatManager,
                        displayExpr: "FullName",
                        valueExpr: 'Id',
                        searchEnabled: true,
                    },

                }, { dataField: 'JobTitle', dataType: 'string', caption: 'Company/Name' }, {
                    dataField: 'ContactNumber', dataType: 'string'
                }];
                var ColumnGridDriver = [{ dataField: 'Id', visible: false }, {
                    dataField: "EmployeeId",
                    caption: "Driver",
                    setCellValue(rowData, value) {
                        var singObj = dxthis.EmployeeData.filter(x => x.Id == value)[0];
                        rowData.EmployeeId = value;
                        rowData.JobTitle = singObj.JobTitle;
                        rowData.ContactNumber = singObj.ContactNumber

                    },
                    lookup: {
                        dataSource: creatManager,
                        displayExpr: "FullName",
                        valueExpr: 'Id',
                        searchEnabled: true,
                    },

                }, { dataField: 'JobTitle', dataType: 'string' }, {
                    dataField: 'ContactNumber', dataType: 'string'
                }];
                var ColumnGridAssesor = [{ dataField: 'Id', visible: false }, {
                    dataField: "EmployeeId",
                    caption: "Assessor",
                    setCellValue(rowData, value) {
                        var singObj = dxthis.EmployeeData.filter(x => x.Id == value)[0];
                        rowData.EmployeeId = value;
                        rowData.JobTitle = singObj.JobTitle;
                        rowData.ContactNumber = singObj.ContactNumber

                    },
                    lookup: {
                        dataSource: creatManager,
                        displayExpr: "FullName",
                        valueExpr: 'Id',
                        searchEnabled: true,
                    },

                }, { dataField: 'JobTitle', dataType: 'string' }, {
                    dataField: 'ContactNumber', dataType: 'string'
                }];

                var ColumnGridCombine = [

                    {
                        dataField: "ContactType",
                        caption: "Contact Type",
                        lookup: {
                            dataSource: ContactType,
                            displayExpr: "FullName",
                            valueExpr: 'Id',
                            searchEnabled: true,
                        },
                    }, {
                        dataField: "ContactId",
                        caption: "Contact Name",
                        setCellValue(rowData, value) {
                            var singObj = dxthis.ContactList.filter(x => x.Id == value)[0];
                            rowData.ContactId = value;
                            rowData.PhoneNumber = singObj.MobileNumber;
                            rowData.Company = singObj.Company;

                        },
                        lookup: {
                            dataSource: dxthis.ContactList,
                            displayExpr: 'Name',
                            valueExpr: 'Id',
                            searchEnabled: true,
                        },
                    }, { dataField: 'PhoneNumber', dataType: 'string' }, { dataField: 'Company', dataType: 'string' }];


                if (gridid == "GridCrewManager") {
                    FinalCol = ColumnGridCrewManager;
                }
                if (gridid == "GridCrewOperator") {
                    FinalCol = ColumnGridCrewOperator;
                }
                if (gridid == "GridConductor") {
                    FinalCol = ColumnGridConductor;
                }
                if (gridid == "GridDriver") {
                    FinalCol = ColumnGridDriver;
                }
                if (gridid == "GridAssesor") {
                    FinalCol = ColumnGridAssesor;
                }
                if (gridid == "GridCombine") {
                    FinalCol = ColumnGridCombine;
                }
                if (gridid == "GridMileStone") {
                    FinalCol = ColumnList;
                }
                bigData = data;


                if (datatype == "new") {
                    bigData = [];
                }



                const datasource = new DevExpress.data.ArrayStore({
                    key: 'Id',
                    data: bigData,
                });

                const dataGrid = $('#' + gridid).dxDataGrid({
                    dataSource: datasource,
                    columns: FinalCol,
                    showBorders: true,
                    showColumnHeaders: true,
                    loadPanel: {
                        enabled: true,
                    },
                    scrolling: {
                        mode: 'virtual',
                    },
                    //columnsAutoWidth: true,
                    focusedRowEnabled: true,
                    width: "100%",
                    paging: {
                        enabled: false,
                    },
                    editing: {

                        mode: gridid == "GridMileStone" ? 'cell' : 'row',
                        allowUpdating: SecurityModel.RoleId != 11,
                        allowDeleting: gridid === "GridMileStone" ? false : (SecurityModel.RoleId != 11),
                        //allowUpdating: true,
                        //allowDeleting: gridid == "GridMileStone" ? false : true,
                        //  allowAdding: datatype == "new" ? false : true,
                        allowAdding: datatype == "new" ? false : (SecurityModel.RoleId == 11 ? false : true),

                        popup: {
                            title: 'Save',
                            showTitle: true,
                            width: 700,
                            height: 300,
                        }
                    },

                    sorting: {
                        mode: "multiple"
                    },
                    allowColumnResizing: true,
                    rowAlternationEnabled: true,
                    columnChooser: {
                        enabled: true,
                        mode: "select",
                        position: {
                            my: 'right top',
                            at: 'right bottom',
                            of: '.dx-datagrid-column-chooser-button',
                        },
                        search: {
                            enabled: true,
                            editorOptions: { placeholder: 'Search column' },
                        },
                        selection: {
                            recursive: true,
                            selectByClick: true,
                            allowSelectAll: true,
                        },
                    },
                    onCellPrepared(e) {

                        if (e.element[0].id == "GridMileStone") {

                            if (e.rowType != "header") {
                                if (e.columnIndex == 0) {
                                    e.cellElement.addClass("color" + e.row.data.LogType);

                                }
                            }

                        }
                    },
                    onRowInserting: function (e) {

                        var IsStaff = false;
                        var url = "/Home/";
                        var paramPost = {};

                        if (e.element[0].id == "GridMileStone") {

                            url = url + "insertGridMileStone"
                            paramPost.ShiftId = dxthis.ShiftId;
                            paramPost.MileStoneEntryDetail = e.data.MileStoneActivity;
                            paramPost.Planned = e.data.Planned;
                            paramPost.Actuall = e.data.Actuall;
                            paramPost.Comments = e.data.Comments;


                        }
                        if (e.element[0].id == "GridCrewManager" || e.element[0].id == "GridCrewOperator" || e.element[0].id == "GridConductor"
                            || e.element[0].id == "GridDriver" || e.element[0].id == "GridAssesor") {
                            IsStaff = true;
                            url = url + "insertPersons"
                            paramPost.ShiftId = dxthis.ShiftId;
                            paramPost.EmployeeId = e.data.EmployeeId;
                            paramPost.EmployeeType = e.element[0].id;
                            paramPost.JobTitle = e.data.JobTitle;
                            paramPost.ContactNumber = e.data.ContactNumber;
                            //if (datetimeperson != undefined && datetimeperson != "" && datetimeperson != null) {
                            //    var date = new Date(datetimeperson);
                            //    var adjustedDate = new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
                            //    var isoString = adjustedDate.toISOString();

                            //    paramPost.DateTimeDetail = isoString;
                            //    console.log(isoString);
                            //}

                            console.log("status", IsStaff)
                        }

                        if (e.element[0].id == "GridCombine") {

                            url = url + "insertShiftContact"
                            paramPost.ShiftId = dxthis.ShiftId;
                            paramPost.ContactType = e.data.ContactType;
                            paramPost.ContactId = e.data.ContactId;

                        }
                        if (IsStaff) {
                            if (e.element[0].id == "GridDriver") {
                                objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                                    if (response != null) {

                                        dxthis.DriverData = response == null ? [] : response.filter(x => x.EmployeeType == "GridDriver");
                                        dxthis.GridModelGrids(response == null ? [] : response.filter(x => x.EmployeeType == "GridDriver"), e.element[0].id);
                                        objCommon.ShowMessage("Record has been Updated", "success");
                                    }
                                    else {
                                        objCommon.ShowMessage(response, "error");
                                    }
                                });
                            }
                            else if (e.element[0].id == "GridConductor") {
                                objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                                    var messageText = "";
                                    if (response == "ShiftExist12")
                                        messageText = "Less than 12 hours since last shift.";
                                    else if (response == "LeaveExist")
                                        messageText = "Staff is scheduled on holiday.";

                                    if (response == "ShiftExist12" || response == "LeaveExist") {
                                        Swal.fire({
                                            title: messageText,
                                            text: "Do you want to add the staff?",
                                            icon: "warning",
                                            showCancelButton: true,
                                            confirmButtonColor: "#3085d6",
                                            cancelButtonColor: "#d33",
                                            confirmButtonText: "Yes!"
                                        }).then((result) => {
                                            if (result.isConfirmed) {
                                                objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                                                    if (response != null) {
                                                        objCommon.ShowMessage("Record has been Updated", "success");
                                                    }
                                                    else {
                                                        objCommon.ShowMessage(response, "error");
                                                    }
                                                });
                                            }
                                        });
                                    }
                                    else {
                                        /* objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response)*/
                                        objCommon.AjaxCall("/Home/CheckIfExist", JSON.stringify(paramPost), "POST", true, function (response) {
                                            if (response != null) {
                                                objCommon.ShowMessage("Record has been Updated", "success");
                                            }
                                            else {
                                                objCommon.ShowMessage(response, "error");
                                            }
                                        });

                                    }

                                });
                            }
                            else {
                                objCommon.AjaxCall("/Home/CheckIfExist", JSON.stringify(paramPost), "POST", true, function (response) {
                                    var messageText = "";
                                    if (response == "ShiftExist")
                                        messageText = "Employee already allocated to shift.";
                                    else if (response == "ShiftExist12")
                                        messageText = "Less than 12 hours since last shift.";
                                    else if (response == "LeaveExist")
                                        messageText = "Staff is scheduled on holiday?";                                                             
                                    if (response == "ShiftExist" || response == "LeaveExist" || response == "ShiftExist12") {
                                        Swal.fire({
                                            title: messageText,
                                            text: "Do you want to add the staff?",
                                            icon: "warning",
                                            showCancelButton: true,
                                            confirmButtonColor: "#3085d6",
                                            cancelButtonColor: "#d33",
                                            confirmButtonText: "Yes!"
                                        }).then((result) => {
                                            if (result.isConfirmed) {
                                                objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                                                    console.log(response);
                                                    if (response != null) {
                                                        objCommon.ShowMessage("Record has been Updated", "success");
                                                    }
                                                    else {
                                                        objCommon.ShowMessage(response, "error");
                                                    }
                                                });
                                            }
                                        });
                                    }
                                    else {
                                        objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                                            if (response != null) {
                                                objCommon.ShowMessage("Record has been Updated", "success");
                                            }
                                            else {
                                                objCommon.ShowMessage(response, "error");
                                            }
                                        });
                                    }

                                });
                            }

                        }
                        else {
                            objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                                console.log(response);
                                if (response != null) {
                                    objCommon.ShowMessage("Record has been Updated", "success");
                                }
                                else {
                                    objCommon.ShowMessage(response, "error");
                                }
                            });
                        }



                    },
                    onRowUpdating: function (e) {
                        //const deferred = $.Deferred();
                        //const promptPromise = DevExpress.ui.dialog.confirm("Are you sure to update record?", "Confirm changes");
                        //promptPromise.done((dialogResult) => {
                        //    if (dialogResult) {

                        //        console.log(e);
                        var url = "/Home/";
                        var paramPost = {};

                        if (e.element[0].id == "GridMileStone") {

                            url = url + "insertGridMileStone"
                            paramPost.ShiftId = dxthis.ShiftId;

                            paramPost.MileStoneEntryDetail = e.newData.MileStoneActivity == undefined ? e.oldData.MileStoneActivity : e.newData.MileStoneActivity;
                            paramPost.Planned = e.newData.Planned == undefined ? e.oldData.Planned : e.newData.Planned;
                            paramPost.Actuall = e.newData.Actuall == undefined ? e.oldData.Actuall : e.newData.Actuall;
                            paramPost.Comments = e.newData.Comments == undefined ? e.oldData.Comments : e.newData.Comments;
                            paramPost.Id = e.oldData.Id;

                        }
                        if (e.element[0].id == "GridCrewManager" || e.element[0].id == "GridCrewOperator" || e.element[0].id == "GridConductor"
                            || e.element[0].id == "GridDriver" || e.element[0].id == "GridAssesor") {

                            url = url + "insertPersons"
                            paramPost.ShiftId = dxthis.ShiftId;
                            paramPost.EmployeeId = e.newData.EmployeeId == undefined ? e.oldData.EmployeeId : e.newData.EmployeeId; //e.newData.EmployeeId;
                            paramPost.EmployeeType = e.element[0].id;
                            paramPost.Id = e.oldData.Id;
                            paramPost.JobTitle = e.newData.JobTitle == undefined ? e.oldData.JobTitle : e.newData.JobTitle;
                            paramPost.ContactNumber = e.newData.ContactNumber == undefined ? e.oldData.ContactNumber : e.newData.ContactNumber;
                            //if (datetimeperson != undefined && datetimeperson != "" && datetimeperson != null) {
                            //    var date = new Date(datetimeperson);
                            //    var adjustedDate = new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
                            //    var isoString = adjustedDate.toISOString();
                            //    paramPost.DateTimeDetail = isoString;
                            //    console.log(isoString);
                            //}
                        }

                        if (e.element[0].id == "GridCombine") {

                            url = url + "insertShiftContact"
                            paramPost.ShiftId = dxthis.ShiftId;
                            paramPost.ContactType = e.newData.ContactType == undefined ? e.oldData.ContactType : e.newData.ContactType;
                            paramPost.ContactId = e.newData.ContactId == undefined ? e.oldData.ContactId : e.newData.ContactId;
                            paramPost.Id = e.oldData.Id;
                        }


                        objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                            console.log(response);
                            if (response != null) {
                                objCommon.ShowMessage("Record has been Updated", "success");
                            }
                            else {
                                objCommon.ShowMessage(response, "error");
                            }
                            // dxthis.Edit(dxthis.d);
                        });

                        //    } else {
                        //        deferred.resolve(true);
                        //     }
                        // });
                        //  e.cancel = deferred.promise();
                    },
                    onRowRemoving: function (e) {

                        console.log(e.data);
                        var url = "/Home/";
                        var paramPost = {
                            Id: e.data.Id
                        };

                        if (e.element[0].id == "GridMileStone") {

                            url = url + "deleteGridMileStone"


                        }
                        if (e.element[0].id == "GridCrewManager" || e.element[0].id == "GridCrewOperator" || e.element[0].id == "GridConductor"
                            || e.element[0].id == "GridDriver" || e.element[0].id == "GridAssesor") {

                            url = url + "deletePersons"

                        }
                        if (e.element[0].id == "GridCombine") {

                            url = url + "deleteShiftContact"

                        }

                        objCommon.AjaxCall(url, JSON.stringify(paramPost), "POST", true, function (response) {
                            console.log(response);

                        });


                    },
                    onContentReady(e) {
                        e.component.option('loadPanel.enabled', false);
                    },
                    toolbar: {
                        items: [
                            gridid == "GridMileStone" ? '' : 'addRowButton',
                            gridid == "GridMileStone" ? 'saveButton' : ''
                        ],
                    },
                    onSelectionChanged(data) {
                        //console.log(data);
                        dataGrid.option('toolbar.items[1].options.disabled', !data.selectedRowsData.length);
                    },
                    onToolbarPreparing: function (e) {
                        if (SecurityModel.RoleId == 11) {
                            return;
                        }
                        e.toolbarOptions.items.unshift({
                            location: "after",
                            widget: "dxButton",
                            visible: gridid == "GridCombine" ? true : false,
                            options: {
                                icon: "edit",
                                hint: "Add Site Contact",
                                onClick: function () {
                                    dxthis.contact = {};
                                    $("#contactModel").modal("show");

                                }
                            }
                        })
                    }
                }).dxDataGrid('instance');
                function getGroupCount(groupField) {
                    return DevExpress.data.query(d.Response.Data.PageInfo)
                        .groupBy(groupField)
                        .toArray().length;

                }
            },
            LogEntry(logentry) {


                var dthis = this;

                objCommon.AjaxCall("/Home/GetLogTypes", $.param({ type: logentry, shiftid: dthis.ShiftId }), "GET", true, function (response) {

                    dthis.LogEntryData = [];
                    response.forEach(function (v, ind) {
                        var childdata = dthis.LogEntryData.filter(x => x.MileStoneActivity == v.LogName && x.LogType == v.LogType)[0];
                        if (childdata != undefined && childdata != null) {

                        }
                        else {
                            dthis.LogEntryData.push({
                                Id: v.Id, MileStoneActivity: v.MileStoneEntryDetail, LogType: v.LogEntry, Planned: v.Planned, Actuall: v.Actuall, Comments: v.Comments
                            });
                        }


                    });

                    dthis.GridModelGrids(dthis.LogEntryData, "GridMileStone");
                })


            },
            ImportFile($event) {
                var dxthis = this;
                var fd = new FormData();

                Files = uppy.getFiles();

                for (var i = 0; i < Files.length; i++) {
                    fd.append(Files[i].meta.name, Files[i].data);
                }
                objCommon.AjaxCallFormData("/Home/ImportFile", fd, false, function (d) {
                    console.log(d);
                    dxthis.ImportData.FilesColName = [];
                    dxthis.FileColumns = d.FileColumnList;
                    if (dxthis.FileColumns != undefined && dxthis.FileColumns != null) {
                        dxthis.FileColumns.forEach(function (v) {
                            dxthis.ImportData.FilesColName.push(v.FilesColName);
                        })
                    }

                    $("#FileUploadeModel").modal("hide");


                    Swal.fire({
                        title: "Are you sure?",
                        text: "Are you sure you want to upload data",
                        icon: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#3085d6",
                        cancelButtonColor: "#d33",
                        confirmButtonText: "Yes, Sure"
                    }).then((result) => {
                        if (result.isConfirmed) {
                            console.log(result);
                            var data = {
                                DBColName: dxthis.ImportData.DBColName,
                                FilesColName: dxthis.ImportData.FilesColName
                            }
                            // $("#divLoader").show();
                            KTApp.showPageLoading();
                            objCommon.AjaxCall("/Home/SaveImportData", JSON.stringify(data), "POST", true, function (response) {
                                KTApp.hidePageLoading();
                                location.reload();
                            });
                        }
                    });


                    //  $("#modelCol").modal("show"); //({ backdrop: 'static', keyboard: false });
                    //setTimeout(function () {

                    //    $("#headercol").sortable({
                    //        update: function (event, ui) {
                    //            var productOrder = $(this).sortable('toArray');//.toString();
                    //            dxthis.ImportData.DBColName = productOrder;

                    //            console.log(dxthis.ImportData);
                    //        }
                    //    });

                    //    $("#filecol").sortable({
                    //        update: function (event, ui) {
                    //            var productOrderfile = $(this).sortable('toArray');//.toString();
                    //            dxthis.ImportData.FilesColName = productOrderfile;
                    //            console.log(dxthis.ImportData);
                    //        }
                    //    });


                    //}, 200)

                });

            },
            ImportFileData($event) {

                console.log(this.ImportData);
                var data = {
                    DBColName: this.ImportData.DBColName,
                    FilesColName: this.ImportData.FilesColName
                }
                objCommon.AjaxCall("/Home/SaveImportData", JSON.stringify(data), "POST", true, function (response) {

                    console.log(response);
                    location.reload();
                }, $event.currentTarget);
            },
            MedicationModel(type) {
                var dxthis = this;
                if (dxthis.DriverData.length > 0) {
                    var empId = dxthis.DriverData[0].EmployeeId;
                    if (type == "dropdown") {
                        empId = Number($("#ddlDriverMedication").val());
                    }

                    objCommon.AjaxCall("/Home/GetDriverQuestion", $.param({ id: dxthis.ShiftId, driverId: empId }), "GET", true, function (response) {
                        console.log(response);
                        dxthis.q = response;
                        dxthis.DriverData.forEach(function (v) {
                            if (v.EmployeeId == response.DriverId) {
                                dxthis.q.DriverName = v.FullName;
                            }
                        })
                        // $("#ddlDriverMedication").val(response.DriverId);

                    })
                    if (type != "dropdown") {
                        $("#DriverMedication").modal("show");
                    }
                }
                else {
                    objCommon.ShowMessage("Please add drivers for this shift.", "error");
                }
            },
            SaveMedication($event) {
                debugger;
                var dxthis = this;
                var dataSend = {};
                console.log(dxthis.q);
                for (key in dxthis.q) {
                    var ekKey = key;
                    key = key.charAt(0).toUpperCase() + key.slice(1);

                    dataSend[key] = dxthis.q[ekKey];

                }

                if (dataSend.Question1 == null || dataSend.Question2 == null || dataSend.Question3 == null || dataSend.Question4 == null
                    || dataSend.Question5 == null || dataSend.Question6 == null || dataSend.Question7 == null || dataSend.Question8 == null
                   /* || dataSend.Question9 == null*/ || dataSend.Question10 == null) {
                    objCommon.ShowMessage("Please answer for all questions.", "error");
                    return;
                }

                if (dataSend.Id == undefined || dataSend.Id == null)
                    dataSend.Id = Number(0);

                objCommon.AjaxCall("/Home/SaveMedication", JSON.stringify(dataSend), "POST", true, function (response) {
                    objCommon.ShowMessage("Saved data successfully.", "success");
                    $("#DriverMedication").modal("hide");
                }, $event.currentTarget)

            },
            ClosePopup() {
                $("#ShiftDetail").modal("hide");
                $("#FileUploadeModel").modal("hide");
                $("#ShiftStatusModel").modal("hide");
                $("#ChangeLogPopup").modal("hide");
            },
            CloseChangeLogPopup() {
                $("#ChangeLogPopup").modal("hide");
            },
            StandardFiltersPopup() {

                $("#StandardFilters").modal("show");
            },
            SearchFilter(type) {

                KTApp.showPageLoading();
                var dxthis = this;
                dxthis.ViewColumns = [];
                //this.search = {};
                this.search.Template = $("#ddlTemplate").val();
                // if (type == "Template") {
                var objTemplate = TemplateData.filter(x => x.Id == this.search.Template)[0];
                if (objTemplate != undefined && objTemplate != null) {
                    if (objTemplate.Columns != undefined && objTemplate.Columns != null) {
                        var coldata = objTemplate.Columns.split(",");
                        coldata.forEach(function (v) {
                            dxthis.ViewColumns.push(v);
                        })
                    }
                }

                //}
                if (type === 'FromWeek') {
                    var selectedFromWeek = $("#ddlFromWeek").val();
                    $("#ddlToWeek").val(selectedFromWeek);
                    //$("#ddlToWeek").empty();               
                    if (this.search.WeekList) {
                        // Populate options of ddlToWeek based on selectedFromWeek
                        this.search.WeekList.forEach(function (temp) {
                            if (temp.Id >= selectedFromWeek) {
                                $("#ddlToWeek").append($('<option>', {
                                    value: temp.Id,
                                    text: temp.Title
                                }));
                            }
                        });
                    }
                    $("#ddlToWeek").val(selectedFromWeek);
                    this.search.ToWeek = $("#ddlToWeek").val();
                }

                if (type === 'ToWeek') {
                    var selectedToWeek = $("#ddlToWeek").val();
                    var selectedFromWeek = $("#ddlFromWeek").val();
                    if (selectedFromWeek > selectedToWeek) {
                        $("#ddlFromWeek").val(selectedFromWeek);
                        //$("#ddlToWeek").empty();               
                        if (this.search.WeekList) {
                            // Populate options of ddlToWeek based on selectedFromWeek
                            this.search.WeekList.forEach(function (temp) {
                                if (temp.Id >= selectedToWeek) {
                                    $("#ddlFromWeek").append($('<option>', {
                                        value: temp.Id,
                                        text: temp.Title
                                    }));
                                }
                            });
                        }
                        $("#ddlFromWeek").val(selectedToWeek);
                        this.search.FromWeek = $("#ddlFromWeek").val();
                    }
                }




                this.search.WorkCompleted = $("#yesNoDropdown").val();
                this.search.MachineNumber = $("#ddlMachineNumber").val();
                this.search.MachineStatus = $("#ddlMachineStatus").val();
                this.search.MachineType = $("#ddlMachineType").val();
                this.search.Staff = $("#ddlStaff").val();
                this.search.LocationSearch = $("#txtLocationSearch").val();
                //this.search.MachineType = $("#ddlMachineType").val();
                this.search.FromWeek = $("#ddlFromWeek").val();
                this.search.ToWeek = $("#ddlToWeek").val();
                this.search.ShiftStatus = $("#ddlShiftStatus").val();
                this.search.Filters = $("#ddlFilters").val();
                if (type == "VRCCToday")
                    this.search.VRCCToday = "VRCCToday";
                else
                    this.search.VRCCToday = "";


                if (type == "All") {
                    this.search.OTM = StadardFilterHistory.OTM == "True" ? true : false;
                    this.search.OTPM = StadardFilterHistory.OTPM == "True" ? true : false;
                    this.search.OTT = StadardFilterHistory.OTT == "True" ? true : false;
                    this.search.ACT = StadardFilterHistory.ACT == "True" ? true : false;
                    this.search.ShiftCaped = StadardFilterHistory.ShiftCaped == "True" ? true : false;
                    this.search.ShiftCancelled = StadardFilterHistory.ShiftCancelled == "True" ? true : false;
                    this.search.ShiftBlank = StadardFilterHistory.ShiftBlank == "True" ? true : false;
                    this.search.Owner = StadardFilterHistory.Owner == "True" ? true : false;
                }


                this.GetData(null, 1, 0);
                this.GetRoster('');
                $("#StandardFilters").modal("hide");
                //dropdownchange filter

            },
            handleEnterKey() {
                // Do something when the enter key is pressed
                this.SearchFilter('LocationFilter');
            },
            GetRoster(dep) {
                var dthis = this;
                var dxthis = this.Roster;

                var roasterdata = {
                    department: $("#ddlMachineDepartment").val(),
                    FromWeek: Number($("#ddlFromWeek").val()),
                    ToWeek: Number($("#ddlToWeek").val())
                }
                objCommon.AjaxCall("/Roster/GetData", $.param(roasterdata), "GET", true, function (response) {

                    dxthis.EmaployeeData = response.EmployeeData;
                    dxthis.DepartmentData = response.DepartmentData;
                    dxthis.RosterData = response.RosterData;
                    var resource = [];
                    var evetns = [];

                    dxthis.EmaployeeData.forEach(function (v) {
                        v.FullName = (v.FirstName == null ? "" : v.FirstName) + ' ' + (v.LastName == null ? "" : v.LastName);


                        resource.push({ id: v.Id, employeeLastName: v.LastName, employee: v.FullName, select: false, leader: false, fitter: false })

                    })

                    var currentWeek = WeekList.filter(x => x.Id == Number($("#ddlFromWeek").val()))[0];
                    var defaultDatetime = "";
                    if (currentWeek != undefined)
                        defaultDatetime = currentWeek.StartDate;

                    dxthis.RosterData.forEach(function (v) {
                        // if (v.EventType == "Leave") {

                        evetns.push({
                            id: v.Id,
                            resourceId: v.EmployeeId,
                            title: v.EventType == 'Leave' ? v.Reason : v.Title,
                            start: v.StartDate,
                            end: v.EndDate,
                            shiftId: v.ShiftId,
                            eventType: v.EventType,
                            //  allDay: true,
                            color: v.Color,     // an option!
                            textColor: 'yellow', // an option!,
                            employeeName: v.EmployeeName,
                            reason: v.Title
                        })
                        //}

                    })

                    var dateOnly = new Date();
                    if (currentWeek != undefined) {

                        // var dateObject = new Date(defaultDatetime);
                        var parts = defaultDatetime.split('/');
                        var day = parseInt(parts[0], 10);
                        var month = parseInt(parts[1], 10) - 1;
                        var year = parseInt(parts[2], 10);

                        var dateObject = new Date(Date.UTC(year, month, day));

                        if (!isNaN(dateObject.getTime())) {
                            //var dateObject = new Date(defaultDatetime + " UTC");

                            //var dateObject = new Date(defaultDatetime);
                            //dateOnly = dateObject.toLocaleDateString("en-GB", { timeZone: "Europe/London" })
                            //var parsedDateObject = new Date(dateOnly.split('/').reverse().join('-'));
                            //console.log("defaultdate", defaultDatetime);
                            dateOnly = dateObject.toLocaleDateString("en-GB", { timeZone: "Europe/London" });
                            //console.log("Date only", dateOnly);
                            //for client
                             var parsedDateObject = new Date(dateObject);
                            // for our use
                           // var parsedDateObject = new Date(dateOnly);

                            //console.log("parsedDate", parsedDateObject);
                            //console.log(dateOnly);
                        } else {

                            console.error('Invalid date:', defaultDatetime);
                        }
                    }

                    var calendarEl = document.getElementById('rosterGrid');
                    calendarEl.innerHTML = "";

                    var calendar = new FullCalendar.Calendar(calendarEl, {
                        schedulerLicenseKey: 'GPL-My-Project-Is-Open-Source',
                        plugins: ['resourceTimeline', 'interaction'],
                        stickyHeaderDates: true,
                        defaultDate: parsedDateObject,
                        header: {
                            left: 'today prev,next',
                            center: 'title',
                            right: 'resourceTimelineDay,resourceTimelineWeek,resourceTimelineMonth'

                        },
                        height: 250,
                        aspectRatio: 1.5,
                        //defaultView: 'resourceTimelineMonth',
                        defaultView: 'resourceTimelineMonth',
                        resourceAreaWidth: '20%',
                        resourceColumns: [
                            {
                                labelText: 'Employees',
                                field: 'employee'
                            }
                        ],
                        firstDay: 6,
                        views: {
                            resourceTimelineWeek: {
                                type: 'resourceTimeline',
                                duration: { weeks: 1 },
                                slotDuration: { days: 1 },

                                weekNumberCalculation: 'local',
                                weekNumberFormat: { week: 'short' },

                            }
                        },

                        resourceRender: function (info) {
                            //  console.log(info.resource.extendedProps);
                            // console.log(info);
                            // console.log(info.el.parentNode);
                            // console.log($(info.el.parentNode.cells[1].querySelector('.fc-cell-text')));
                            // console.log(info.el.querySelector('.fc-cell-text'));
                            //var questionMark = document.createElement('input');
                            //questionMark.type = 'checkbox';
                            //questionMark.id = "select_" + info.resource.id;
                            //questionMark.className = "checkchange";
                            //questionMark.name = "select_" + info.resource.id;
                            //// questionMark.checked = true;
                            //$(questionMark).attr('checked', info.select);



                            //var questionMark2 = document.createElement('input');
                            //questionMark2.type = 'checkbox';
                            //questionMark2.className = "checkchange";
                            //questionMark2.id = "leader_" + info.resource.id;
                            //questionMark2.name = "leader_" + info.resource.id;
                            //$(questionMark2).attr('checked', info.leader);

                            //var questionMark3 = document.createElement('input');
                            //questionMark3.type = 'checkbox';
                            //questionMark3.id = "fitter_" + info.resource.id;
                            //questionMark3.className = "checkchange";
                            //questionMark3.name = "fitter_" + info.resource.id;
                            //$(questionMark3).attr('checked', info.fitter);


                            //$(info.el.parentNode.cells[1].querySelector('.fc-cell-text')
                            //).html(questionMark);
                            //$(info.el.parentNode.cells[2].querySelector('.fc-cell-text')
                            //).html(questionMark2);
                            //$(info.el.parentNode.cells[3].querySelector('.fc-cell-text')
                            //).html(questionMark3);


                        },
                        resourceOrder: 'employeeLastName',
                        resources: resource,
                        events: evetns,

                        eventRender: function (info) {
                            // console.log(info);
                            //console.log("infostart", info.event.start);
                            $(info.el).tooltip({ title: info.event.title, start: info.event.start, end: info.event.end });
                            // console.log("title", info.event.title);

                            //  console.log("what is titel",info.event.title)
                            // var eventContent = '<div class="fc-title">' + info.event.title + '</div>' +
                            //   '<div class="fc-tooltip-data" data-start="' + info.event.start + '" data-end="' + info.event.end + '">Hover for Details</div>';

                            // Set the event's HTML content
                            // $(info.el).tooltip(eventContent);


                            // var eventContent = '<div class="fc-title">' + info.event.title + '</div>' +
                            //    '<div class="fc-time">' + info.event.start + ' - ' + info.event.end + '</div>';

                            // Set the event's HTML content
                            // $(info.el).html(eventContent);

                            // console.log(event.title);
                            // Access custom properties and use them to customize the event rendering
                            // event.el.querySelector('.fc-title').append('<br>' + event.title);
                        },
                        eventClick: function (info) {
                            // console.log(info);
                            // console.log(info.event.start, info.event.end);
                            // console.log(info.event.extendedProps.eventType);
                            console.log(info.event.extendedProps.shiftId);
                            if (info.event.extendedProps.eventType == "Shift") {


                                objCommon.AjaxCall("/Home/GetShiftDataById", $.param({ shiftId: info.event.extendedProps.shiftId }), "GET", true, function (dresponse) {
                                    console.log(dresponse);
                                    dthis.RosterShift = {};
                                    dthis.RosterShift.DriverName = info.event.extendedProps.employeeName;
                                    dthis.RosterShift.ShiftLocation = dresponse.WorksiteDetails;
                                    dthis.RosterShift.Description = "";

                                    const dateObject = new Date(dresponse.StartDateTime);
                                    const finishdateObject = new Date(dresponse.FinishDateTime);
                                    if (!isNaN(dateObject)) {
                                        dthis.RosterShift.StartDateTime = moment(dateObject).format("DD-MM-YYYY HH:MM");
                                        dthis.RosterShift.FinishDateTime = moment(finishdateObject).format("DD-MM-YYYY HH:MM");
                                        //    // Format the Date object to a string in "YYYY-MM-DD" format
                                        //    //  data.StartDateTime = dateObject.toISOString().split('T')[0];

                                    }

                                    dthis.RosterShift.ShiftId = info.event.extendedProps.shiftId;

                                    //dxthis.RosterShift.FromActuall = dresponse.FirstName;
                                    // dxthis.RosterShift.ToActuall = dresponse.FirstName;

                                    $("#shiftModel").modal("show");
                                })

                            }
                            else {
                                var fday = ("0" + info.event.start.getDate()).slice(-2);
                                var fmonth = ("0" + (info.event.start.getMonth() + 1)).slice(-2);

                                var tday = ("0" + info.event.end.getDate()).slice(-2);
                                var tmonth = ("0" + (info.event.end.getMonth() + 1)).slice(-2);

                                var ftoday = (fday) + "-" + (fmonth) + "-" + info.event.start.getFullYear();
                                var ttoday = (tday) + "-" + (tmonth) + "-" + info.event.end.getFullYear();
                                dthis.Absance.FromDate = ftoday;
                                dthis.Absance.ToDate = ttoday;
                                dthis.Absance.LeaveType = info.event.extendedProps.reason;
                                dthis.Absance.Reason = info.event.title;
                                $("#absanceModel").modal("show");
                            }
                            // console.log(jsEvent);
                            // console.log(view);
                        }
                        , dayRender: function (date) {
                            // Check if the day is Saturday (6) or Sunday (0)
                            //if (date.getDay() === 6 || date.getDay() === 0) {
                            var dayName = new Date(date.date).toLocaleDateString('en-US', { weekday: 'long' });
                            if (dayName == "Saturday" || dayName == "Sunday")
                                date.el.style.backgroundColor = '#f1e0e0'; // Change the background color
                            //  cell.css('color', 'white'); // Change the text color
                            // }
                        }
                    });


                    calendar.render();

                })
            },
            SaveDescription($event) {
                var dxthis = this;
                var desData = {
                    ShiftId: dxthis.RosterShift.ShiftId,
                    RosterShiftDescription: dxthis.RosterShift.Description
                }
                objCommon.AjaxCall("/Roster/SaveShiftRosterDescription", JSON.stringify(desData), "POST", true, function (d) {
                    console.log(d);
                    if (d.Value == "success") {
                        objCommon.ShowMessage("Description Updated Successfully.", "success");
                        $("#shiftModel").modal("hide");
                    }
                    else {
                        objCommon.ShowMessage(d, "error");
                    }

                }, $event.currentTarget)
            },
            OpenShiftDetail() {
                var dxthis = this;
                console.log("shiftdata", dxthis.ShiftData)
                var shiftDetail = dxthis.ShiftData.filter(x => x.Id == dxthis.RosterShift.ShiftId)[0];
                console.log("shift", shiftDetail);
                dxthis.ActionType = "Update";
                dxthis.Edit(shiftDetail);
            },
            GetAbsance(data) {
                var dxthis = this.Absance;
                var json = JSON.parse(data);
                //console.log(json);
                dxthis.EmployeeId = json.id;


                var datadb = {
                    PageIndex: 1,
                    PageSize: 100,
                    SortColumn: "DateFrom",
                    SortOrder: "desc",
                    SearchText: "text",
                    EmployeeId: dxthis.EmployeeId
                }

                objCommon.AjaxCall("/Abs/GetData", $.param(datadb), "GET", true, function (response) {

                    var columnNames = [
                        { dataField: 'DateFromString', caption: 'Date From', dataType: 'date' },
                        { dataField: 'DateToString', caption: 'Date To', dataType: 'date' },
                        { dataField: 'LeaveType', caption: 'Type', dataType: 'string' }, { dataField: 'Reason', caption: 'Description', dataType: 'string' }];
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
                            // dxthis.Edit(e.data);
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
            },
            DeleteAbsance($event) {
                var et = this.Absance;
                var param = {
                    Id: et.AbsId
                };

                objCommon.AjaxCall("/Abs/delete", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Deleted.", "success");
                }, $event.currentTarget);
            },
            InsertAbsance($event) {
                var et = this.Absance;
                var param = et.d;
                param.EmployeeId = et.EmployeeId;
                objCommon.AjaxCall("/Abs/insert", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Added.", "success");
                    et.GetData(null, 1);
                }, $event.currentTarget);

            },
            CloseShiftPopup() {
                $("#shiftModel").modal("hide");
            },
            CloseAbsancePopup() {
                $("#absanceModel").modal("hide");
            },
            ShowVSTPPopup() {

                this.VSTP = {};
                this.VSTP.ShiftId = this.d.Id;
                this.VSTP.OriginLoc = this.d.OutShortCode;
                this.VSTP.OriginName = this.d.OutName;
                this.VSTP.OriginStanox = this.d.InStanox;
                this.VSTP.OriginTime = this.d.StartTime;
                this.VSTP.DestLoc = this.d.InShortCode;
                this.VSTP.DestName = this.d.InName;
                this.VSTP.DestStanox = this.d.OutStanox;
                this.VSTP.DestTime = this.d.FinishTime;
                this.VSTP.HeadCode = this.d.HeadCode;
                this.VSTP.Comments = "";
                this.VSTP.NumberOfVehicles = "ONE";

                $("#VSTPPopup").modal("show");
            },
            ChangeLogPopup(type) {
                $("#ChangeLogPopup").modal("show");
                $("#btnAddLog").show();
                $("#btnExportLog").show();
                $("#h2Heading").html("Change Log");
                var dxthis = this;
                objCommon.AjaxCall("/ChangeLog/GetChangeLog", $.param({ ShiftId: dxthis.ShiftId }), "GET", true, function (response) {
                    console.log(response);
                    var ColumnName = [{
                        dataField: "LogShiftDate", caption: "Log Date", dataType: 'date'
                        , format: "dd-MM-yyyy"
                    }, {
                        dataField: "ChangeDate", caption: "Change Date", dataType: 'date'
                        , format: "dd-MM-yyyy"
                    }, {
                        dataField: "InstigatedBy", caption: "Instegated"
                    }, {
                        dataField: "ChangePeriod", caption: "Change Period"
                    }, {
                        dataField: "Ptonumber", caption: "PTO Number"
                    }, {
                        dataField: "ChangedBy", caption: "Changed By"
                    }, {
                        dataField: "ContactName", caption: "Contact Name"
                    }, {
                        dataField: "ChangeType", caption: "ChangeType"
                    }, {
                        dataField: "FurtherAction", caption: "Further Action"
                    }, {
                        dataField: "MoreInformation", caption: "Change Details"
                    },
                    {
                        dataField: "MachineNum", caption: "Machine Num"
                    }
                    ]
                    const employeesStore = new DevExpress.data.ArrayStore({
                        key: 'Id',
                        data: response,
                    });
                    var dataGrid = $('#changeloggrid').dxDataGrid({

                        dataSource: employeesStore,
                        columns: ColumnName,
                        showBorders: true,
                        loadPanel: {
                            enabled: true,
                        },
                        scrolling: {
                            columnRenderingMode: 'virtual',
                        },
                        columnsAutoWidth: true,
                        paging: {
                            enabled: true,
                            pageSize: 15,
                            pageIndex: 1
                        },
                        sorting: {
                            mode: "multiple"
                        },
                        allowColumnResizing: true,
                        rowAlternationEnabled: true,
                    }).dxDataGrid('instance');


                });

            },
            AuditLogPopup(type) {

                $("#ChangeLogPopup").modal("show");
                $("#btnAddLog").hide();
                $("#btnExportLog").hide();
                $("#h2Heading").html("Audit Log");
                var dxthis = this;
                objCommon.AjaxCall("/Home/GetChangeLog", $.param({ ShiftId: dxthis.ShiftId }), "GET", true, function (response) {
                    var ColumnName = [{
                        dataField: "CreatedDateString", caption: "Date/By",
                        //calculateCellValue(data) {
                        //    return data['CreatedDateString'] + '\r\n' + data['FullName'];
                        //},
                        format: "dd-MM-yyyy HH:mm",
                        width: "20%",
                        cellTemplate: function (container, options) {
                            var div = document.createElement('div');
                            div.innerHTML = options.data.CreatedDateString + "</br>" + options.data.FullName; // Change 'columnName' to the actual data field name
                            container.append(div);
                        }
                    }, {
                        dataField: "ActionType", caption: "Action Type", width: "10%"
                    }, {
                        dataField: "Description", caption: "Description", width: "100%"
                    }
                        //, {
                        //dataField: "FullName", caption: "User Name"
                        //}
                    ]
                    const employeesStore = new DevExpress.data.ArrayStore({
                        key: 'Id',
                        data: response,
                    });
                    setTimeout(function () {
                        var dataGrid = $('#changeloggrid').dxDataGrid({
                            dataSource: employeesStore,
                            columns: ColumnName,
                            showBorders: true,
                            loadPanel: {
                                enabled: true,
                            },
                            scrolling: {
                                columnRenderingMode: 'virtual',
                            },
                            columnsAutoWidth: true,
                            paging: {
                                enabled: true,
                                pageSize: 15,
                                pageIndex: 1
                            },
                            sorting: {
                                mode: "multiple"
                            },
                            allowColumnResizing: true,
                            rowAlternationEnabled: true,
                        }).dxDataGrid('instance');

                        //$(window).resize(function () {
                        //    dataGrid.repaint();
                        //});
                    }, 100); 
                });

                
            },
            ChangeShortCode(vstp, type) {

                var dxthis = this.VSTP;

                if (type == "OriginLoc") {
                    var inforoutes = this.RoutesList.filter(x => x.ShortCode == vstp)[0];
                    console.log(inforoutes);
                    dxthis.OriginName = inforoutes.Name + '';
                    dxthis.OriginStanox = inforoutes.Stanox + '';
                }
                else {

                    var inforoutes = this.RoutesList.filter(x => x.ShortCode == vstp)[0];
                    console.log(inforoutes);
                    dxthis.DestName = inforoutes.Name + '';
                    dxthis.DestStanox = inforoutes.Stanox + '';
                }

                this.$forceUpdate();

            },
            SaveVSTPSDetail($event) {
                var et = this;
                var param = et.VSTP;
                param.Id = 0;
                objCommon.AjaxCall("/Home/InsertVSTP", JSON.stringify(param), "POST", true, function (response) {
                    if (response == "Updated") {

                        objCommon.ShowMessage("Added.", "success");

                        var urlss = "/Reports/Index?Type=VSTP&ShiftId=" + param.ShiftId;
                        window.open(
                            urlss,
                            '_blank' // <- This is what makes it open in a new window.
                        );
                    }
                    else {
                        objCommon.ShowMessage(response, "error");
                    }



                }, $event.currentTarget);

            },
            SaveShiftStatus($event) {
                var et = this;

                var param = {
                    ShiftStatus: $("#ddlShiftStatus2").val(),
                    Ids: et.SelectedShiftData
                }
                objCommon.AjaxCall("/Home/SaveShiftStatus", JSON.stringify(param), "POST", true, function (response) {
                    if (response == "Updated") {

                        objCommon.ShowMessage("Added.", "success");
                        location.reload();
                    }
                    else {
                        objCommon.ShowMessage(response, "error");
                    }



                }, $event.currentTarget);
            },
            WeeklyComemntsPopup() {

                $("#WeeklyComments").modal("show");
            },
            SubmitComment($event) {
                var et = this;
                if ($("#ddlFromWeek").val() == "" || $("#ddlFromWeek").val() == undefined || $("#ddlFromWeek").val() == null) {
                    objCommon.ShowMessage("Please select week.", "error");
                    return;
                }

                this.comment.WeekId = Number($("#ddlFromWeek").val());
                var param = this.comment;
                objCommon.AjaxCall("/Home/SaveWeeklyComments", JSON.stringify(param), "POST", true, function (response) {
                    if (response == "Saved") {

                        objCommon.ShowMessage("Added.", "success");
                    }
                    else {
                        objCommon.ShowMessage(response, "error");
                    }



                }, $event.currentTarget);
            },
            AddItems(dd) {

                this.comment.ListArrangements.push(this.item);
                this.item = {};

            },
            DeleteItem(item) {
                //   this.comment.CommentsData(item);

                this.comment.ListArrangements = $.grep(this.comment.ListArrangements, function (obj) {
                    return JSON.stringify(obj) !== JSON.stringify(item);
                });

            },
            SendEmail($event) {

                var emailAddress = "";
                var bodyValue = "";
                var EmailArray = new Array();
                if (this.SelectedDistData.length > 0) {
                    this.SelectedDistData.forEach(function (v) {
                        EmailArray.push(v.EmailAddress);

                    })
                    if (EmailArray) {
                        emailAddress = EmailArray.join('; ');
                    }
                }

                console.log(this.ShiftRowData);

                for (key in this.ShiftRowData) {
                    if (key == "Ptonumber" || key == "WeekNo" || key == "MachineNum" || key == "Id" || key == "WorksiteDetails") {
                        if (key == "WorksiteDetails") {
                            bodyValue = bodyValue + "Site" + " : " + this.ShiftRowData[key] + " ,";
                        }
                        else {
                            bodyValue = bodyValue + (key == "Id" ? "MUF" : key) + " : " + this.ShiftRowData[key] + " ,";
                        }

                    }
                    if (key == "StartDateTime" || key == "FinishDateTime") {
                        const dateObject = new Date(this.ShiftRowData[key]);
                        if (!isNaN(dateObject)) {
                            this.ShiftRowData[key] = moment(dateObject).format("DD-MM-YYYY");
                            bodyValue = bodyValue + (key == "StartDateTime" ? "Start Date" : (key == "FinishDateTime") ? "End Date" : "Date") + " : " + moment(dateObject).format("DD-MM-YYYY") + " ,";
                        }
                    }
                }

                var emailAddresses = emailAddress; // Add your email addresses separated by commas
                var subject = "PLANT AMENDMENT CHANGE";
                var body = bodyValue;

                var mailtoLink = "mailto:" + emailAddresses +
                    "?subject=" + encodeURIComponent(subject) +
                    "&body=" + encodeURIComponent(body);
                window.location.href = mailtoLink;
            },
            OpenOutlook() {


                var subject = "PRISM System Access";
                var body = "";

                var mailtoLink = "mailto:otmplanning@volkerrail.co.uk" +
                    "?subject=" + encodeURIComponent(subject) +
                    "&body=" + encodeURIComponent(body);
                window.location.href = mailtoLink;
            },
            ResetFilters($event) {

                var et = this;

                objCommon.AjaxCall("/Home/ResetFilters", JSON.stringify({}), "POST", true, function (response) {
                    if (response == "Deleted") {

                        location.reload();
                    }
                    else {
                        objCommon.ShowMessage(response, "error");
                    }

                }, $event.currentTarget);

            },
            WeeklyRosterReport() {
                window.location.href = "/Reports/Index?Type=PLANTCREWROSTERREPORT&ShiftId=0&FromWeek=" + $("#ddlFromWeek").val() + "&ToWeek=" + $("#ddlToWeek").val();
            },
            ChangeMachineNumber(machinenum) {
                var objMachine = MachineList.filter(x => x.MachineNum == machinenum)[0];
                this.d.MachineType = objMachine.MachineType;
                this.d.HeadCode = objMachine.HeadCode;
            },
            calculationHours() {

                var dxthis = this.d;
                // Convert the time strings to Date objects
                var startDate = new Date('1970-01-01T' + dxthis.StartTime + 'Z');
                var endDate = new Date('1970-01-01T' + dxthis.FinishTime + 'Z');

                // Calculate the time difference in milliseconds
                var timeDifference = endDate - startDate;

                // Convert the time difference to hours
                var hours = timeDifference / (1000 * 60 * 60);
                dxthis.PprePlannedHours = hours;
                // Display the result
                //  document.getElementById('result').innerText = 'Hours difference: ' + hours.toFixed(2) + ' hours';
            },
            ExportAllRecords($event) {
                var dxthis = this;
                // $("#DriverMedication").modal("show");
                dxthis.StandardFilters = [];
                dxthis.StandardFilters.push({ FieldValue: 'OTM', Caption: 'Omit MUF Recieved', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'OTPM', Caption: 'Omit Log Printed', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'OTT', Caption: 'Omit Manager AH', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ACT', Caption: 'Omit Workshops', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ShiftBlank', Caption: 'Omit Blanks', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ShiftCaped', Caption: 'Omit Caped', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'ShiftCancelled', Caption: 'Omit Cancelled', Checked: false });
                dxthis.StandardFilters.push({ FieldValue: 'Owner', Caption: 'OmitHire-in Machine', Checked: false });
                //dxthis.StandardFilters.push({ FieldValue: '', Caption: '', Checked: false });
                var pageNum = localStorage.getItem("PageIndex");
                if (pageNum != undefined && pageNum != "" && pageNum != null) {
                    num = pageNum;
                }


                dxthis.search.PageIndex = num;
                dxthis.search.PageSize = 10000;
                dxthis.search.SortColumn = "Id";
                dxthis.search.SortOrder = "desc";
                dxthis.search.SearchText = "";


                objCommon.AjaxCall("/Home/GetExcelExport", $.param(dxthis.search), "GET", true, function (response) {
                    console.log(response);
                    var urlss = "/ShiftLists/" + response.Value;
                    window.open(
                        urlss,
                        '_blank' // <- This is what makes it open in a new window.
                    );
                }, $event == null ? null : $event.currentTarget);
            },
            AddChangeLog() {
                $("#ModelAddChangeLog").modal("show");
                var dxthis = this;
                dxthis.log = {};
                dxthis.log.PTONumber = dxthis.d.Ptonumber;
                dxthis.log.MachineNum = dxthis.d.MachineNum;
                //console.log(log.MachineNum);
                var currentDate = new Date();
                var day = currentDate.getDate();
                var month = currentDate.getMonth() + 1; // January is 0, so we need to add 1
                var year = currentDate.getFullYear();

                // Pad day and month with leading zeros if needed
                day = day < 10 ? '0' + day : day;
                month = month < 10 ? '0' + month : month;

                var formattedDate = day + '-' + month + '-' + year;
                dxthis.log.ChangeDate = formattedDate;
                dxthis.log.LogShiftDate = dxthis.d.StartDateTime;

                //$("#txtShiftDate").datepicker({
                //    dateFormat: 'dd-mm-yy',//check change
                //    defaultDate: currentDate,
                //    onSelect: function (dateText, inst) {
                //        dxthis.log.LogShiftDate = dateText;

                //    }
                //});
                $("#txtChangeDate").datepicker({
                    dateFormat: 'dd-mm-yy',//check change
                    defaultDate: currentDate,
                    onSelect: function (dateText, inst) {
                        dxthis.log.ChangeDate = dateText;
                    }
                });

            },
            SaveChangeLog($event) {
                var dxthis = this;

                dxthis.log.ChangedBy = $("#txtChangedBy").val();
                dxthis.log.ShiftId = dxthis.ShiftId;
                objCommon.AjaxCall("/ChangeLog/insert", JSON.stringify(dxthis.log), "POST", true, function (response) {
                    $("#ModelAddChangeLog").modal("hide");
                    dxthis.ChangeLogPopup();

                }, $event == null ? null : $event.currentTarget);

            },
            ExportChangeLog($event) {
                var dxthis = this;
                objCommon.AjaxCall("/ChangeLog/ExportExcel", $.param({ ShiftId: dxthis.ShiftId }), "GET", true, function (response) {
                    console.log(response);
                    var urlss = "/ShiftLists/" + response.Value;
                    window.open(
                        urlss,
                        '_blank' // <- This is what makes it open in a new window.
                    );
                });
            },
            InsertContact($event) {
                if (!objCommon.Validate("#inputform"))
                    return;


                var param = this.contact;
                var dxthis = this;
                objCommon.AjaxCall("/Contacts/insert", JSON.stringify(param), "POST", true, function (response) {
                    console.log(response);
                    objCommon.ShowMessage("Added.", "success");
                    dxthis.ContactList = response;
                    dxthis.GridModelGrids(dxthis.ContactTypeObject == null ? [] : dxthis.ContactTypeObject, "GridCombine", "update");
                    $("#contactModel").modal("hide");

                }, $event.currentTarget);
            },
            CloseContactPopup() {
                $("#contactModel").modal("hide");
            }
        },
        created() {

            this.SearchFilter("All");
            // this.GetData(null, 1);
            //this.GetRoster('');
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
            var dxthis = this;
            $(".timechangeEvent").change(function (e) {

                var startDate = new Date('1970-01-01T' + $("#floatingStartTime").val() + 'Z');
                var endDate = new Date('1970-01-01T' + ($("#floatingFinishTime").val() == "" ? "00:00" : $("#floatingFinishTime").val()) + 'Z');

                // Calculate the time difference in milliseconds
                var timeDifference = endDate - startDate;
                if (timeDifference < 0) {
                    // Add a day (in milliseconds) to the time difference
                    timeDifference += 24 * 60 * 60 * 1000;
                }
                // Convert the time difference to hours
                var hours = timeDifference / (1000 * 60 * 60);
                var totalHours = Math.floor(hours);
                var minutes = Math.round((hours - totalHours) * 60);

                // Display the result in "hh:mm" format
                var durationInHHMM = pad(totalHours) + ':' + pad(minutes);
                dxthis.d.PprePlannedHours = durationInHHMM;
                // 
                $("#floatingPlannedHours").val(durationInHHMM);

                var DateTimeMix = dxthis.d.StartDateTime + " " + $("#floatingStartTime").val();
                console.log(DateTimeMix);
                objCommon.AjaxCall("/Home/GetShiftDayWeek", $.param({ DateTimeMix: DateTimeMix }), "GET", true, function (response) {
                    if (response != null) {
                        dxthis.d.ShiftNo = response.ShiftNumber;
                        if (response.WeekNumber != undefined && response.WeekNumber != "" && response.WeekNumber != null)
                            dxthis.d.WeekNo = response.WeekNumber.split(' ')[2];
                        dxthis.$forceUpdate();
                    }
                })

            })

            function pad(number) {
                return (number < 10 ? '0' : '') + number;
            }

            $("#btnSelectedRecordExport").dxButton({

                onClick: () => {
                    var workbook = new ExcelJS.Workbook();
                    var worksheet = workbook.addWorksheet("LNEDetail");

                    DevExpress.excelExporter.exportDataGrid({
                        component: $("#gridContainer").dxDataGrid("instance"),
                        worksheet: worksheet,
                        autoFilterEnabled: true,
                    }).then(function () {
                        workbook.xlsx.writeBuffer().then(function (buffer) {
                            saveAs(new Blob([buffer], { type: "application/octet-stream" }), "selecteddataGrid.xlsx");
                        });
                    });
                }
            });

            $("#btnExportLog").dxButton({
                onClick: () => {
                    var workbook = new ExcelJS.Workbook();
                    var worksheet = workbook.addWorksheet("ChangeLog");

                    DevExpress.excelExporter.exportDataGrid({
                        component: $("#changeloggrid").dxDataGrid("instance"),
                        worksheet: worksheet,
                        autoFilterEnabled: true,
                    }).then(function () {
                        workbook.xlsx.writeBuffer().then(function (buffer) {
                            saveAs(new Blob([buffer], { type: "application/octet-stream" }), "ChangeLogData.xlsx");
                        });
                    });
                }
            });

        }
    })


    var uppy = Uppy.Core()
        .use(Uppy.Dashboard, {
            inline: true,
            target: '#drag-drop-area',
            // target: 'body',
            metaFields: [
                { id: 'name', name: 'Name', placeholder: 'file name' },
                { id: 'caption', name: 'Caption', placeholder: 'describe what the image is about' }
            ],
            width: "100%",
            height: 200,
            maxNumberOfFiles: 1,
            showProgressDetails: true,
            hideUploadButton: true,
            doneButtonHandler: () => {
                this.uppy.reset()
                this.requestCloseModal()
            },
            proudlyDisplayPoweredByUppy: false,
            theme: 'light'
        })


    uppy.on('complete', (result) => {
        console.log('Upload complete! We’ve uploaded these files:', result.successful);
    });
    uppy.on('file-added', (file) => {
        console.log(file);
        Files.push(file);
    });
    uppy.on('file-edit', (file) => {
        console.log(file);
        Files.push(file);
    });
    uppy.on('file-removed', (file, reason) => {
        console.log('Removed file', file);
    });

    uppy.on('info-visible', () => {
        const { info } = uppy.getState();

        console.log(`${info.message} ${info.details}`);
    });
    uppy.on('restriction-failed', (file, error) => {
        // do some customized logic like showing system notice to users
    });
    uppy.on('file-added', (file) => {
        console.log('Added file', file);
    })
    //$("#btnAllRecordExport").dxButton({
    //    onClick: () => {
    //        var workbook = new ExcelJS.Workbook();
    //        var worksheet = workbook.addWorksheet("LNEDetail");

    //        DevExpress.excelExporter.exportDataGrid({
    //            component: $("#gridContainer").dxDataGrid("instance"),
    //            worksheet: worksheet,
    //            autoFilterEnabled: true,
    //        }).then(function () {
    //            workbook.xlsx.writeBuffer().then(function (buffer) {
    //                saveAs(new Blob([buffer], { type: "application/octet-stream" }), "AlldataGrid.xlsx");
    //            });
    //        });
    //    }
    //});

})







