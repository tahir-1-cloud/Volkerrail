var objCommon = new Common();
$(document).ready(function () {
    $(".tableformat tr td").attr("style", "border-right-style: solid;" +
        "border-right-width: 0.75pt;" +
        "border-left-style: solid; " +
        "border-left-width: 0.75pt; " +
        "border-bottom-style: solid; " +
        "border-bottom-width: 0.75pt; " +
        "padding-right: 5.03pt; " +
        "padding-left: 5.03pt;" +
        "vertical-align: top; ");
    $(".tableformat2 tr td").attr("style", "border-right-style: solid;" +
        "border-right-width: 0.75pt;" +
        "border-left-style: solid; " +
        "border-left-width: 0.75pt; " +
        "border-bottom-style: solid; " +
        "border-bottom-width: 0.75pt; " +
        "padding-right: 5.03pt; " +
        "padding-left: 5.03pt;" +
        "vertical-align: top; ");
    $(".tableformat6 tr td").attr("style", "border-right-style: solid;" +
        "border-right-width: 0.75pt;" +
        "border-left-style: solid; " +
        "border-left-width: 0.75pt; " +
        "border-bottom-style: solid; " +
        "border-bottom-width: 0.75pt; " +
        "padding-right: 5.03pt; " +
        "padding-left: 5.03pt;" +
        "vertical-align: top;height:20px ");

    var app = new Vue({
        el: '#apporg',
        data: {
            DataList: [],
            SelectedDistData: [],
            d: {}
        },
        methods: {
            Delete(id) {
                var param = {
                    Id: id
                };

                objCommon.AjaxCall("/Contacts/delete", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Deleted.", "success");
                    location.reload();
                });
            },
            Edit(data) {
                var json = JSON.parse(data);
                this.d = {};
                //this.d=;
                this.d.Name = json.name;
                this.d.Id = json.id;
                this.d.Company = json.company;
                this.d.PhoneNumber = json.phoneNumber;
                this.d.FaxNumber = json.faxNumber;
                this.d.MobileNumber = json.mobileNumber;
                this.d.Email = json.email;
                this.d.Notes = json.notes;
                this.d.JobTitle = json.jobTitle;
                this.d.Address = json.address;

                $("#lookupmodel").modal("show");
            },
            ShowLookup($event) {
                this.d = {};
                this.d.Id = 0;
                $("#lookupmodel").modal("show");
            },
            Insert($event) {
                var param = this.d;

                objCommon.AjaxCall("/Contacts/insert", JSON.stringify(param), "POST", true, function (response) {
                    objCommon.ShowMessage("Added.", "success");

                    $("#lookupmodel").modal("hide");
                    location.reload();
                }, $event.currentTarget);

            },
            GetData($event) {


                objCommon.AjaxCall("/Contacts/GetData", $.param({ Type }), "GET", true, function (response) {
                }, $event == null ? null : $event.currentTarget);

            },
            ClosePopup() {
                $("#lookupmodel").modal("hide");
            },
            GenerateReport(type, $event) {

                if (type == "BOXREPORT") {

                    if ($("#hdWeekNumber").val() == "") {
                        alert("Please Generate the report first.");
                        return;

                    }

                    var data = {
                        ReportHTML: $("#pdfcontent").html(),
                        ReportType: type,
                        FromWeek: $("#ddlFromWeek").val(),
                        Department: $("#ddlMachineDepartments").val(),
                        WeekNumber: $("#hdWeekNumber").val(),
                        WeekDateTime: $("#hdWeekDateTime").val()
                    }

                    objCommon.AjaxCall("/Reports/GenerateBOXReport", JSON.stringify(data), "POST", true, function (response) {
                        var urlss = "/ReportsPDF/" + response.Value;
                        window.open(
                            urlss,
                            '_blank' // <- This is what makes it open in a new window.
                        );
                    }, $event == null ? null : $event.currentTarget);
                }
                else {
                    var data = {
                        ReportHTML: $("#pdfcontent").html(),
                        ReportType: type
                    }
                    objCommon.AjaxCall("/Reports/GenerateReport", JSON.stringify(data), "POST", true, function (response) {
                        var urlss = "/ReportsPDF/" + response.Value;
                        window.open(
                            urlss,
                            '_blank' // <- This is what makes it open in a new window.
                        );
                    }, $event == null ? null : $event.currentTarget);
                }

            },
            SearchReport(type, $event) {
                var ToWeek = "";
                var Department = "";
                if (type != "BOXREPORT" && type != "PLANTCREWROSTERREPORT") {
                    ToWeek = $("#ddlToWeek").val();
                }
                if (type == "BOXREPORT") {
                    Department = $("#ddlMachineDepartments").val();
                }

                window.location.href = "/Reports/Index?Type=" + type + "&ShiftId=0&FromWeek=" + $("#ddlFromWeek").val() + "&ToWeek=" + ToWeek + "&Dep=" + Department;
            },
            GetDistributionList(type, $event) {
                var dxthis = this;
                objCommon.AjaxCall("/Reports/GetDistributionList", $.param({}), "GET", true, function (response) {
                    console.log(response);
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
                }, $event.currentTarget);

            },
            SendEmail(type, $event) {

                var dxthis = this;
                var data = {
                    ReportHTML: $("#pdfcontent").html(),
                    ReportType: type
                }
                objCommon.AjaxCall("/Reports/GenerateReport", JSON.stringify(data), "POST", true, function (response) {
                    var urlss = "https://" + window.location.hostname + "/ReportsPDF/" + response.Value;
                    var emailAddress = "";
                    var bodyValue = "";
                    if (dxthis.SelectedDistData.length > 0) {
                        dxthis.SelectedDistData.forEach(function (v) {
                            emailAddress = emailAddress + v.EmailAddress + ",";
                        })
                    }


                    var emailAddresses = emailAddress; // Add your email addresses separated by commas
                    var subject = "Weekly Roster Report";
                    var body = "Weekly Roster Report " + urlss;

                    var mailtoLink = "mailto:" + emailAddresses +
                        "?subject=" + encodeURIComponent(subject) +
                        "&body=" + body;
                    window.location.href = mailtoLink;


                }, $event == null ? null : $event.currentTarget);


            },
            SearchChangeLogReport($event) {
                var data = {
                    FromWeek: $("#ddlFromWeek").val(),
                    ToWeek: $("#ddlToWeek").val()
                }
                objCommon.AjaxCall("/ChangeLog/GetChangeLogByDate", $.param(data), "GET", true, function (response) {
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
                }, $event == null ? null : $event.currentTarget);
            },
            ExportWeeklyRoster($event) {
                var data = {
                    Type: "",
                    FromWeek: $("#ddlFromWeek").val(),
                    ToWeek: 0
                }
                objCommon.AjaxCall("/Reports/ExportWeeklyRoster", $.param(data), "GET", true, function (response) {
                    var urlss = "/ReportsPDF/" + response.Value;
                    window.open(
                        urlss,
                        '_blank' // <- This is what makes it open in a new window.
                    );
                }, $event == null ? null : $event.currentTarget);
            }
        },
        created() {
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

            $(".datepickerchangelog").datepicker({
                dateFormat: 'dd-mm-yy'
            });
        }
    })


    $(".nav-item").removeClass("active");
    $("#liLookups").addClass("active");
    $(".menu-item").removeClass("show");
    $(".menu-item").removeClass("here");
    $("#menuContacts").addClass("here");
    $("#menuContacts").addClass("show");
    $(".tab-pane").removeClass("active");
    $(".tab-pane").removeClass("show");
    $("#tabLookups").addClass("active");
    $("#tabLookups").addClass("show");

    $("#btnAllRecordExportChangeLog").dxButton({
        onClick: () => {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet("ChangeLog");

            DevExpress.excelExporter.exportDataGrid({
                component: $("#changeloggrid").dxDataGrid("instance"),
                worksheet: worksheet,
                autoFilterEnabled: true,
            }).then(function () {
                workbook.xlsx.writeBuffer().then(function (buffer) {
                    saveAs(new Blob([buffer], { type: "application/octet-stream" }), "AllChangeLog.xlsx");
                });
            });
        }
    });


    //var pdf = new jsPDF('p', 'pt', 'letter')

    //    // source can be HTML-formatted string, or a reference
    //    // to an actual DOM element from which the text will be scraped.
    //    , source = $('#pdfcontent')[0]

    //    // we support special element handlers. Register them with jQuery-style
    //    // ID selector for either ID or node name. ("#iAmID", "div", "span" etc.)
    //    // There is no support for any other type of selectors
    //    // (class, of compound) at this time.
    //    , specialElementHandlers = {
    //        // element with id of "bypass" - jQuery style selector
    //        '#bypassme': function (element, renderer) {
    //            // true = "handled elsewhere, bypass text extraction"
    //            return true
    //        }
    //    }

    //margins = {
    //    top: 80,
    //    bottom: 60,
    //    left: 40,
    //    width: 522
    //};
    //// all coords and widths are in jsPDF instance's declared units
    //// 'inches' in this case
    //pdf.fromHTML(
    //    source // HTML string or DOM elem ref.
    //    , margins.left // x coord
    //    , margins.top // y coord
    //    , {
    //        'width': margins.width // max width of content on PDF
    //        , 'elementHandlers': specialElementHandlers
    //    },
    //    function (dispose) {
    //        // dispose: object with X, Y of the last line add to the PDF
    //        //          this allow the insertion of new lines after html
    //        pdf.save('Test.pdf');
    //    },
    //    margins
    //)


})
