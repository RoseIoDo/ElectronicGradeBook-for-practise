$(document).ready(function () {

    loadSubjects();

    $("#addSubjectBtn").click(function () {
        clearForm();
        $("#subjectModalLabel").text("Додати предмет");
        $("#subjectModal").modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#subjectModalLabel").text("Редагувати предмет");

        $("#subjectId").val($(this).data("id"));
        $("#fullName").val($(this).data("fname"));
        $("#shortName").val($(this).data("sname"));
        $("#code").val($(this).data("code"));
        $("#isElective").prop("checked", $(this).data("elective") == true);
        $("#cycleType").val($(this).data("cycle"));

        $("#subjectModal").modal("show");
    });

    $("#saveSubjectBtn").click(function () {
        const idVal = parseInt($("#subjectId").val()) || 0;
        const fnameVal = $("#fullName").val().trim();
        const snameVal = $("#shortName").val().trim();
        const codeVal = $("#code").val().trim();
        const electVal = $("#isElective").is(":checked");
        const cycleVal = $("#cycleType").val().trim();

        if (!fnameVal) {
            alert("Повна назва не може бути пустою.");
            return;
        }

        const url = idVal ? "/Subjects/Edit" : "/Subjects/Add";
        const dataObj = {
            id: idVal,
            fullName: fnameVal,
            shortName: snameVal,
            code: codeVal,
            isElective: electVal,
            cycleType: cycleVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#subjectModal").modal("hide");
                    loadSubjects();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження предмету.");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити предмет?")) {
            $.ajax({
                url: `/Subjects/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadSubjects();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення.");
                }
            });
        }
    });

    function loadSubjects() {
        $.ajax({
            url: "/Subjects/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#subjectsTable tbody");
                $tbody.empty();
                list.forEach(s => {
                    $tbody.append(`
                        <tr>
                            <td>${s.id}</td>
                            <td>${s.fullName}</td>
                            <td>${s.shortName}</td>
                            <td>${s.code || ""}</td>
                            <td>${s.isElective ? "Так" : "Ні"}</td>
                            <td>${s.cycleType || ""}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${s.id}"
                                    data-fname="${s.fullName}"
                                    data-sname="${s.shortName}"
                                    data-code="${s.code}"
                                    data-elective="${s.isElective}"
                                    data-cycle="${s.cycleType}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                    data-id="${s.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження предметів.");
            }
        });
    }

    function clearForm() {
        $("#subjectId").val("");
        $("#fullName").val("");
        $("#shortName").val("");
        $("#code").val("");
        $("#isElective").prop("checked", false);
        $("#cycleType").val("");
    }

    function loadFilteredSubjects() {
        let search = $("#subjectSearch").val() || "";
        let isElectiveVal = $("#electiveSelect").val(); // "All"/"True"/"False"

        let isElective = null;
        if (isElectiveVal === "True") isElective = true;
        else if (isElectiveVal === "False") isElective = false;

        $.ajax({
            url: "/Subjects/GetFiltered",
            type: "GET",
            data: {
                search: search,
                isElective: isElective,
                pageNumber: currentPage,
                pageSize: pageSize
            },
            success: function (resp) {
                if (!resp.success) {
                    alert(resp.message);
                    return;
                }
                const paged = resp.data;
                renderSubjectsTable(paged.items);
                // ...
            },
            error: function () {
                alert("Помилка фільтрації предметів.");
            }
        });
    }

});
