import React from "react";
import Header from "../components/Header";
import { useState, useRef } from "react";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import { AlertMessage, AlertError } from "../components/Alert";

function NewUser() {
  const axiosPrivate = useAxiosPrivate();
  const usernameRef = useRef();
  const [username, setUsername] = useState("");
  const passwordRef = useRef();
  const [password, setPassword] = useState("");
  const confirmPasswordRef = useRef();
  const [confirmPassword, setConfirmPassword] = useState("");
  const roleRef = useRef();
  const [role, setRole] = useState("");
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [errors, setErrors] = useState({});
  const [message, setMessage] = useState({
    show: false,
    text: "",
  });
  const [error, setError] = useState({
    show: false,
    text: "",
  });

  const validateForm = () => {
    setErrors({});

    let isValid = true;

    if (username.trim() === "") {
      setErrors((prevErrors) => ({
        ...prevErrors,
        username: "Username is required",
      }));
      isValid = false;
    }

    if (password.trim() === "") {
      setErrors((prevErrors) => ({
        ...prevErrors,
        password: "Password is required",
      }));
      isValid = false;
    }

    if (confirmPassword.trim() === "") {
      setErrors((prevErrors) => ({
        ...prevErrors,
        confirmPassword: "Confirm Password is required",
      }));
      isValid = false;
    } else if (confirmPassword !== password) {
      setErrors((prevErrors) => ({
        ...prevErrors,
        confirmPassword: "Passwords do not match",
      }));
      isValid = false;
    }

    if (role.trim() === "") {
      setErrors((prevErrors) => ({ ...prevErrors, role: "Role is required" }));
      isValid = false;
    }

    return isValid;
  };

  const handleSubmit = async () => {
    setIsSubmitted(true);
    if (!validateForm()) {
      console.log(errors);
      return;
    }
    console.log("Form submitted");
    const controller = new AbortController();
    try {
      const response = await axiosPrivate.post(
        `/user/register`,
        JSON.stringify({
          Username: username,
          Password: password,
          Role: role,
        }),
        {
          signal: controller.signal,
        }
      );
      console.log(response.data);
      setMessage({
        show: true,
        text: "User has been created successfully.",
      });
    } catch (error) {
      console.error(error);
      setError({
        show: true,
        text: JSON.stringify(error.response.data.errors),
      });
    }
  };

  const handleCloseMessage = () => {
    setMessage({ show: false, text: "" });
  };

  const handleCloseError = () => {
    setError({ show: false, text: "" });
  };

  return (
    <>
      <Header />
      {message.show && (
        <AlertMessage message={message.text} handleClose={handleCloseMessage} />
      )}
      {error.show && (
        <AlertError message={error.text} handleClose={handleCloseError} />
      )}
      <form
        onSubmit={(e) => {
          e.preventDefault();
          handleSubmit();
        }}
      >
        <div className="mb-3">
          <label htmlFor="Username" className="form-label">
            Username:
          </label>
          <input
            type="text"
            id="Username"
            ref={usernameRef}
            autoComplete="off"
            onChange={(e) => setUsername(e.target.value)}
            value={username}
            required
            className="form-control"
          />
          {isSubmitted && errors.username && (
            <div className="form-text">{errors.username}</div>
          )}
        </div>
        <div className="mb-3">
          <label htmlFor="Password" className="form-label">
            Password:
          </label>
          <input
            type="password"
            id="Password"
            ref={passwordRef}
            autoComplete="off"
            onChange={(e) => setPassword(e.target.value)}
            value={password}
            required
            className="form-control"
          />
          {isSubmitted && errors.password && (
            <div className="form-text">{errors.password}</div>
          )}
          <label htmlFor="ConfirmPassword" className="form-label">
            Confirm Password:
          </label>
          <input
            type="password"
            id="ConfirmPassword"
            ref={confirmPasswordRef}
            autoComplete="off"
            onChange={(e) => setConfirmPassword(e.target.value)}
            value={confirmPassword}
            required
            className="form-control"
          />
          {isSubmitted && errors.confirmPassword && (
            <div className="form-text">{errors.confirmPassword}</div>
          )}
        </div>
        <div className="mb-3">
          <label htmlFor="Role" className="form-label">
            Role:
          </label>
          <input
            type="text"
            id="Role"
            ref={roleRef}
            autoComplete="off"
            onChange={(e) => setRole(e.target.value)}
            value={role}
            required
            className="form-control"
          />
          {isSubmitted && errors.role && (
            <div className="form-text">{errors.role}</div>
          )}
        </div>
        <button className="btn btn-primary">Create</button>
      </form>
    </>
  );
}

export default NewUser;
