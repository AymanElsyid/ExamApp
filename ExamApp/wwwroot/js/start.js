document.addEventListener("DOMContentLoaded", function () {
    // Function to get examId from URL
    function getExamIdFromUrl() {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get("Id"); // Extract examId from query string
    }

    // Fetch and display exam data
    async function fetchExamData() {
        try {
            const examId = getExamIdFromUrl(); // Get examId from URL
            if (!examId) {
                console.error("No examId found in URL");
                return;
            }
            let response = await fetch(`/exam/getExamData?Id=${examId}`);
            if (!response.ok) {
                throw new Error("Failed to fetch exam details.");
            }
            let data = await response.json();
            displayExamData(data);
        } catch (error) {
            Swal.fire("Error!", error.message, "error");
        }
    }

    // Function to render questions dynamically
    function displayExamData(examData) {
        document.getElementById("Title").textContent = examData.examTitle;
        document.getElementById("Title1").textContent = examData.examTitle;
        const container = document.getElementById("questions-container");
        container.innerHTML = ""; // Clear any existing content

        examData.questions.forEach((q, index) => {
            const section = document.createElement("section");
            section.classList.add("question-section", "mb-4", "p-3", "border", "rounded");

            // Question Title
            section.innerHTML = `
                <h3>Question ${index + 1}</h3>
                <p><strong>${q.questionTitle}</strong></p>
            `;

            // Create answers as radio buttons
            q.answers.forEach((answer, answerIndex) => {
                const div = document.createElement("div");
                div.classList.add("pt-3");

                div.innerHTML = `
        <input class="form-check-input" type="radio"
            name="${q.id}" 
            id="answer-${q.id}-${answerIndex}"  
            value="${answer.id}">
        <label class="form-check-label" for="answer-${q.id}-${answerIndex}">
            ${answer.answerTitle}
        </label>
    `;

                section.appendChild(div);
            });


            container.appendChild(section);
        });

        // Add Submit Button
        const submitButton = document.createElement("button");
        submitButton.textContent = "Submit Exam";
        submitButton.classList.add("btn", "btn-primary", "mt-3");
        submitButton.onclick = submitExam;
        container.appendChild(submitButton);
    }

    // Function to get selected answers and submit
    function submitExam(event) {
        event.preventDefault(); // Prevent default form submission

        const examId = getExamIdFromUrl();
        if (!examId) {
            Swal.fire("Error!", "Exam ID is missing.", "error");
            return;
        }

        const questions = [];
        let unansweredQuestions = [];

        document.querySelectorAll(".question-section").forEach((section, index) => {
            const questionId = section.querySelector("input[type='radio']").name;
            const selectedAnswer = section.querySelector("input[type='radio']:checked");

            if (selectedAnswer) {
                questions.push({
                    questionId: questionId,
                    answerId: selectedAnswer.value
                });
            } else {
                unansweredQuestions.push(index + 1); // Store question number for error message
            }
        });

        // If any question is unanswered, stop submission and show alert
        if (unansweredQuestions.length > 0) {
            Swal.fire(
                "Warning!",
                `Please answer all questions before submitting. Unanswered Questions: ${unansweredQuestions.join(", ")}`,
                "warning"
            );
            return;
        }

        // Prepare data in ExamSubmit format
        const examSubmitData = {
            examId: examId,
            questions: questions
        };

        console.log("Submitting Data:", examSubmitData); // Debugging

        fetch("/exam/submitExam", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(examSubmitData)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Submission failed.");
                }
                return response.json();
            })
            .then(data => {
                Swal.fire("Success!", "Exam submitted successfully!", "success").then(() => {
                    window.location.href = "/exam/index"; // Redirect after submission
                });
            })
            .catch(error => {
                Swal.fire("Error!", "Failed to submit the exam.", "error");
                console.error("Error submitting exam:", error);
            });
    }

    // Fetch exam data when the page loads
    fetchExamData();
});
