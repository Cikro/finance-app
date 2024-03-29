/* See https://www2.1010data.com/documentationcenter/prime/1010dataUsersGuide/DataTypesAndFormats/currencyUnitCodes.html */
CREATE TABLE IF NOT EXISTS currencies
(
    code char(3) PRIMARY KEY NOT NULL,
    name varchar(255) NOT NULL,
    date_created TIMESTAMP NOT NULL default CURRENT_TIMESTAMP, -- Remove OnUpdate from default Timestamp Behaviour.
    date_last_edited TIMESTAMP on update CURRENT_TIMESTAMP     -- Remove initial default from Timestamp when record is first created.
);

CREATE TABLE IF NOT EXISTS account_types 
(
    id tinyint unsigned PRIMARY KEY AUTO_INCREMENT,
    name varchar(50) NOT NULL,
    date_created TIMESTAMP NOT NULL default CURRENT_TIMESTAMP,
    date_last_edited TIMESTAMP on update CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS accounts 
(
    id int unsigned PRIMARY KEY AUTO_INCREMENT,
    user_id int unsigned NOT NULL,
    name varchar(50) NOT NULL UNIQUE,
    description varchar(255),
    balance DECIMAL(15,2) default 0,
    type tinyint unsigned NOT NULL,
    currency_code char(3) NOT NULL,
    parent_account int unsigned default NULL,
    closed BOOLEAN default FALSE NOT NULL, 
    date_created TIMESTAMP NOT NULL default CURRENT_TIMESTAMP,
    date_last_edited TIMESTAMP on update CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
        ON DELETE CASCADE ON UPDATE RESTRICT,
    FOREIGN KEY (type) REFERENCES account_types(id)
        ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (currency_code) REFERENCES currencies(code)
        ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (parent_account) REFERENCES accounts(id)
        ON DELETE RESTRICT ON UPDATE RESTRICT -- Stop DELETE if the account has children. If you need to delete the account, remove its children first. 
);

/* If an insert sets the account balance, set the balance to zero */
CREATE TRIGGER IF NOT EXISTS setAccountBalanceZero
    BEFORE INSERT ON accounts
    FOR EACH ROW
    SET NEW.balance = 0;
