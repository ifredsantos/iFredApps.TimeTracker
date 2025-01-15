CREATE DATABASE IF NOT EXISTS db_time_tracker;

USE db_time_tracker;

CREATE TABLE users (
    user_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(60) NOT NULL,
    name VARCHAR(120) NOT NULL,
    email VARCHAR(150) NOT NULL,
    password VARCHAR(100) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT NOW()
);

CREATE TABLE sessions (
    session_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    workspace_id INT NOT NULL,
    start_date DATETIME NOT NULL,
    end_date DATETIME NULL,
    description VARCHAR(255) NOT NULL,
    observation VARCHAR(500) NULL,
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);

CREATE TABLE workspaces (
    workspace_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    name VARCHAR(60) NOT NULL,
    is_default BOOLEAN NOT NULL,
    created_at DATETIME NOT NULL DEFAULT NOW(),
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);