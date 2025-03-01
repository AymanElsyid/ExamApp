document.addEventListener("DOMContentLoaded", function () { 
    let questionIndex = 0;
    let modal = document.getElementById("questionPopup");
    let closeModal = document.querySelector(".close");
    let answers = [];
    let editingQuestion = null;

    // Initialize TinyMCE
    tinymce.init({
        selector: '#questionText',
        height: 500,
        theme: 'silver',
        plugins: [
            'advlist autolink lists link image charmap print preview hr anchor pagebreak',
            'searchreplace wordcount visualblocks visualchars code fullscreen',
            'insertdatetime media nonbreaking save table contextmenu directionality',
            'emoticons template paste textcolor colorpicker textpattern imagetools codesample toc help'
        ],
        toolbar1: 'undo redo | insert | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',
        toolbar2: 'print preview media | forecolor backcolor emoticons | codesample help',
        image_advtab: true,
        templates: [{
            title: 'Test template 1',
            content: 'Test 1'
        },
        {
            title: 'Test template 2',
            content: 'Test 2'
        }
        ],
        content_css: []
    });

    document.getElementById("addQuestion").addEventListener("click", function () {
        let examTitle = document.getElementById("examTitle").value.trim();
        if (!examTitle) {
            Swal.fire('Warning!', 'Please enter the exam title.', 'warning');
            return;
        }
        openQuestionPopup();
    });

    function openQuestionPopup(questionData = null) {
        tinymce.get("questionText").setContent(questionData ? questionData.text : "");
        document.getElementById("answersContainer").innerHTML = "";
        answers = [];
        editingQuestion = questionData;

        if (questionData) {
            questionData.answers.forEach(ans => addAnswer(ans.text, ans.isCorrect));
        }

        modal.style.display = "block";
    }

    closeModal.addEventListener("click", function () {
        modal.style.display = "none";
        editingQuestion = null;
    });

    document.getElementById("addAnswer").addEventListener("click", function () {
        let questionText = tinymce.get("questionText").getContent().trim();

        if (!questionText) {
            Swal.fire('Warning!', 'Please enter a question before adding answers.', 'warning');
            return;
        }
            
        let lastAnswer = answers.length > 0 ? answers[answers.length - 1].value.value.trim() : "";
        if (lastAnswer === "" && answers.length > 0) {
            Swal.fire('Warning!', 'Please fill in the previous answer before adding a new one.', 'warning');
            return;
        }
        if (answers.length >= 4) {
            Swal.fire('Warning!', 'You can only add up to 4 answers per question.', 'warning');
            return;
        }
        addAnswer();
    });

    function addAnswer(text = "", isCorrect = false) {
        let answerDiv = document.createElement("div");
        answerDiv.classList.add("answer", "row", "mt-2");
        answerDiv.style.display = 'flex';

        let input = document.createElement("textarea");
        input.classList.add("answerTitle","mr-1", "col-6");
        input.required = true;
        input.value = text;

        let correctCheckbox = document.createElement("input");
        correctCheckbox.type = "checkbox";
        correctCheckbox.classList.add("mr-1", "correctAnswer", "col-1");
        correctCheckbox.style.borderRadius = '70%';
        correctCheckbox.checked = isCorrect;

        correctCheckbox.addEventListener("change", function () {
            answers.forEach(a => a.correctAnswer.checked = false);
            correctCheckbox.checked = true;
        });

        let removeBtn = document.createElement("button");
        removeBtn.type = "button";
        removeBtn.classList.add("badge", "badge-danger", "badge-pill", "col-2");
        removeBtn.textContent = "Remove";

        removeBtn.addEventListener("click", function () {
            answers = answers.filter(a => a !== answerData);
            answerDiv.remove();
        });

        let answerData = { value: input, correctAnswer: correctCheckbox };
        answers.push(answerData);

        answerDiv.appendChild(input);
        answerDiv.appendChild(correctCheckbox);
        answerDiv.appendChild(removeBtn);
        document.getElementById("answersContainer").appendChild(answerDiv);
    }

    document.getElementById("saveQuestion").addEventListener("click", function () {
        let questionText = tinymce.get("questionText").getContent().trim();
        if (!questionText) {
            Swal.fire('Warning!', 'Please enter a question.', 'warning');
            return;
        }
        let validAnswers = answers.filter(a => a.value.value.trim() !== "");


        if (validAnswers.length < 2) {
            Swal.fire('Warning!', 'Each question must have at least two answers.', 'warning');
            return;
        }



        let hasCorrectAnswer = validAnswers.some(a => a.correctAnswer.checked);
        if (!hasCorrectAnswer) {
            Swal.fire('Warning!', 'Each question must have at least one correct answer.', 'warning');
            return;
        }

        if (editingQuestion) {
            editingQuestion.label.innerHTML = Question: ${questionText};
            editingQuestion.answersList.innerHTML = "";
            validAnswers.forEach(answer => {
                let li = document.createElement("li");
                li.textContent = answer.value.value + (answer.correctAnswer.checked ? " (Correct)" : "");
                editingQuestion.answersList.appendChild(li);
            });
        } else {
            addNewQuestion(questionText);
        }

        modal.style.display = "none";
        editingQuestion = null;
    });

    function addNewQuestion(questionText) {
        let questionDiv = document.createElement("div");
        questionDiv.classList.add("question","py-2");
        questionDiv.dataset.questionIndex = questionIndex;

        let questionLabel = document.createElement("label");
        questionLabel.innerHTML = Question: ${questionText};

        let answerList = document.createElement("ul");
        answers = answers.filter(a => a.value.value.trim() !== "");
        answers.forEach(answer => {
            let li = document.createElement("li");
            li.textContent = answer.value.value + (answer.correctAnswer.checked ? " (Correct)" : "");
            answerList.appendChild(li);
        });
        //Need Edit Style
        let editBtn = document.createElement("button");
        editBtn.type = "button";
        editBtn.classList.add("btn", "btn-warning", "btn-sm", "mx-2");
        editBtn.textContent = "Edit";
        editBtn.addEventListener("click", function () {
            openQuestionPopup({ text: questionText, answers: answers.map(a => ({ text: a.value.value, isCorrect: a.correctAnswer.checked })), label: questionLabel, answersList: answerList });
        });

        let deleteBtn = document.createElement("button");
        deleteBtn.type = "button";
        deleteBtn.classList.add("btn", "btn-danger", "btn-sm","mx-2");
        deleteBtn.textContent = "Delete";
        deleteBtn.addEventListener("click", function () {
            Swal.fire({
                title: "Are you sure?",
                text: "You won't be able to revert this!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#3085d6",
                confirmButtonText: "Yes, delete it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    questionDiv.remove();
                    Swal.fire("Deleted!", "Your question has been deleted.", "success");
                }
            });
        });

        questionDiv.appendChild(questionLabel);
        questionDiv.appendChild(answerList);
        questionDiv.appendChild(editBtn);
        questionDiv.appendChild(deleteBtn);
        document.getElementById("questionsContainer").appendChild(questionDiv);

        questionIndex++;
    }

    document.getElementById("examForm").addEventListener("submit", function (event) {
        event.preventDefault();

        let examTitle = document.getElementById("examTitle").value.trim();
        if (!examTitle) {
            Swal.fire('Warning!', 'Please enter the exam title.', 'warning');
            return;
        }

        let questions = [];
        document.querySelectorAll(".question").forEach(questionDiv => {
            let questionText = questionDiv.querySelector("label").textContent.replace(/^Question: /, "").trim();
            let answers = [];
            questionDiv.querySelectorAll("ul li").forEach(li => {
                let isCorrect = li.textContent.includes("(Correct)");
                let answerText = li.textContent.replace(" (Correct)", "").trim();
                answers.push({ AnswerTitle: answerText, IsCorrect: isCorrect });
            });

            questions.push({ QuestionTitle: questionText, Answers: answers });
        });

        if (questions.length === 0) {
            Swal.fire('Warning!', 'Please add at least one question.', 'warning');
            return;
        }

        let examData = {
            ExamTitle: examTitle,
            ShowInHomePage: document.getElementById("showInHomePage").checked,
            Questions: questions
        };

        fetch('/Admin/Exam/CreateExam', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(examData)
        })
            .then(() => Swal.fire('Success!', 'Exam has been added successfully!', 'success'))
            .catch(() => Swal.fire('Error!', 'Something went wrong.', 'error'));
    });
});