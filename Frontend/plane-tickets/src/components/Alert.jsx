import React from "react";

export const AlertMessage = ({ message, handleClose }) => {
  return (
    <div className="alert alert-success alert-dismissible" role="alert">
      <div>{message}</div>
      <button
        type="button"
        className="btn-close"
        data-bs-dismiss="alert"
        aria-label="Close"
        onClick={handleClose}
      />
    </div>
  );
};

export const AlertError = ({ message, handleClose }) => {
  return (
    <div className="alert alert-danger alert-dismissible" role="alert">
      <div>{message}</div>
      <button
        type="button"
        className="btn-close"
        data-bs-dismiss="alert"
        aria-label="Close"
        onClick={handleClose}
      />
    </div>
  );
};
