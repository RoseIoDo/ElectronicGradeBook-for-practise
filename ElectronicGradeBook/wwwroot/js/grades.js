$(document).ready(function () {
    loadGrades();
    loadDropdownData(); // якщо треба відображати списки subjectOfferings, students

    $("#addGradeBtn").click(function () {
        clearForm();
        $("#gradeModalLabel").text("Додати оцінку");
        $("#gradeModal").modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#gradeModalLabel").text("Редагувати оцінку");

        $("#gradeId").val($(this).data("id"));
        $("#offeringSelect").val($(this).data("offid"));
        $("#studentSelect").val($(this).data("studid"));
        $("#gradeVersionJson").val($(this).data("json"));
        $("#status").val($(this).data("status"));
        $("#isRetake").prop("checked", $(this).data("retake") == true);

        $("#gradeModal").modal("show");
    });

    $("#saveGradeBtn").click(function () {
        const idVal = parseInt($("#gradeId").val()) || 0;
        const offVal = parseInt($("#offeringSelect").val()) || 0;
        const studVal = parseInt($("#studentSelect").val()) || 0;
        const jsonVal = $("#gradeVersionJson").val().trim() || "{}";
        const statusVal = $("#status").val().trim() || "";
        const retakeVal = $("#isRetake").is(":checked");

        const url = idVal ? "/Grades/Edit" : "/Grades/Add";
        const dataObj = {
            id: idVal,
            subjectOfferingId: offVal,
            studentId: studVal,
            gradeVersionJson: jsonVal,
            status: statusVal,
            isRetake: retakeVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#gradeModal").modal("hide");
                    loadGrades();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження оцінки.");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити оцінку?")) {
            $.ajax({
                url: `/Grades/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadGrades();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення оцінки.");
                }
            });
        }
    });

    function loadGrades() {
        $.ajax({
            url: "/Grades/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#gradesTable tbody");
                $tbody.empty();
                list.forEach(g => {
                    const dateStr = g.dateUpdated ? g.dateUpdated.split("T")[0] : "";
                    $tbody.append(`
                        <tr>
                            <td>${g.id}</td>
                            <td>${g.subjectName}</td>
                            <td>${g.studentName}</td>
                            <td>${g.status || ""}</td>
                            <td>${g.isRetake ? "Так" : "Ні"}</td>
                            <td>${dateStr}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${g.id}"
                                    data-offid="${g.subjectOfferingId}"
                                    data-studid="${g.studentId}"
                                    data-json="${g.gradeVersionJson || "{}"}"
                                    data-status="${g.status || ""}"
                                    data-retake="${g.isRetake}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                    data-id="${g.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження оцінок.");
            }
        });
    }

    function loadDropdownData() {
        // /SubjectOfferings/GetAll, /Students/GetAll => fill #offeringSelect, #studentSelect
    }

    function clearForm() {
        $("#gradeId").val("");
        $("#offeringSelect").val("");
        $("#studentSelect").val("");
        $("#gradeVersionJson").val("");
        $("#status").val("");
        $("#isRetake").prop("checked", false);
    }
});
