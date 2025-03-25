import React from "react";
import ReactDOM from "react-dom/client";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

window.showToast = (message, type = "info") => {
    toast[type](message);
};

const App = () => (
    <>
        <ToastContainer position="top-right" autoClose={3000} />
    </>
);

ReactDOM.createRoot(document.getElementById("root")).render(<App />);

