-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

CREATE SCHEMA "adminapp_HangFire";

SET default_tablespace = '';

SET default_with_oids = false;

CREATE TABLE "adminapp_HangFire".counter (
    id bigint NOT NULL,
    key varchar NOT NULL,
    value bigint NOT NULL,
    expireat timestamp without time zone
);

CREATE SEQUENCE "adminapp_HangFire".counter_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".counter_id_seq OWNED BY "adminapp_HangFire".counter.id;

CREATE TABLE "adminapp_HangFire".hash (
    id bigint NOT NULL,
    key varchar NOT NULL,
    field varchar NOT NULL,
    value varchar,
    expireat timestamp without time zone,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE SEQUENCE "adminapp_HangFire".hash_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".hash_id_seq OWNED BY "adminapp_HangFire".hash.id;

CREATE TABLE "adminapp_HangFire".job (
    id bigint NOT NULL,
    stateid bigint,
    statename varchar,
    invocationdata varchar NOT NULL,
    arguments varchar NOT NULL,
    createdat timestamp without time zone NOT NULL,
    expireat timestamp without time zone,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE SEQUENCE "adminapp_HangFire".job_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".job_id_seq OWNED BY "adminapp_HangFire".job.id;

CREATE TABLE "adminapp_HangFire".jobparameter (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name varchar NOT NULL,
    value varchar,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE SEQUENCE "adminapp_HangFire".jobparameter_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".jobparameter_id_seq OWNED BY "adminapp_HangFire".jobparameter.id;

CREATE TABLE "adminapp_HangFire".jobqueue (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    queue varchar NOT NULL,
    fetchedat timestamp without time zone,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE SEQUENCE "adminapp_HangFire".jobqueue_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".jobqueue_id_seq OWNED BY "adminapp_HangFire".jobqueue.id;

CREATE TABLE "adminapp_HangFire".list (
    id bigint NOT NULL,
    key varchar NOT NULL,
    value varchar,
    expireat timestamp without time zone,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE SEQUENCE "adminapp_HangFire".list_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".list_id_seq OWNED BY "adminapp_HangFire".list.id;

CREATE TABLE "adminapp_HangFire".lock (
    resource varchar NOT NULL,
    updatecount int DEFAULT 0 NOT NULL,
    acquired timestamp without time zone
);

CREATE TABLE "adminapp_HangFire".schema (
    version int NOT NULL
);

CREATE TABLE "adminapp_HangFire".server (
    id varchar NOT NULL,
    data varchar,
    lastheartbeat timestamp without time zone NOT NULL,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE TABLE "adminapp_HangFire".set (
    id bigint NOT NULL,
    key varchar NOT NULL,
    score double precision NOT NULL,
    value varchar NOT NULL,
    expireat timestamp without time zone,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE SEQUENCE "adminapp_HangFire".set_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".set_id_seq OWNED BY "adminapp_HangFire".set.id;

CREATE TABLE "adminapp_HangFire".state (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name varchar NOT NULL,
    reason varchar,
    createdat timestamp without time zone NOT NULL,
    data varchar,
    updatecount int DEFAULT 0 NOT NULL
);

CREATE SEQUENCE "adminapp_HangFire".state_id_seq
    AS int
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE "adminapp_HangFire".state_id_seq OWNED BY "adminapp_HangFire".state.id;

ALTER TABLE ONLY "adminapp_HangFire".counter ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".counter_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".hash ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".hash_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".job ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".job_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".jobparameter ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".jobparameter_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".jobqueue ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".jobqueue_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".list ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".list_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".set ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".set_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".state ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".state_id_seq'::regclass);

ALTER TABLE ONLY "adminapp_HangFire".counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);

ALTER TABLE ONLY "adminapp_HangFire".hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);

ALTER TABLE ONLY "adminapp_HangFire".schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);

ALTER TABLE ONLY "adminapp_HangFire".server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);

ALTER TABLE ONLY "adminapp_HangFire".set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);

ALTER TABLE ONLY "adminapp_HangFire".state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);

CREATE INDEX ix_hangfire_counter_expireat ON "adminapp_HangFire".counter USING btree (expireat);

CREATE INDEX ix_hangfire_counter_key ON "adminapp_HangFire".counter USING btree (key);

CREATE INDEX ix_hangfire_job_statename ON "adminapp_HangFire".job USING btree (statename);

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON "adminapp_HangFire".jobparameter USING btree (jobid, name);

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON "adminapp_HangFire".jobqueue USING btree (jobid, queue);

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON "adminapp_HangFire".jobqueue USING btree (queue, fetchedat);

CREATE INDEX ix_hangfire_state_jobid ON "adminapp_HangFire".state USING btree (jobid);

ALTER TABLE ONLY "adminapp_HangFire".jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES "adminapp_HangFire".job(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY "adminapp_HangFire".state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES "adminapp_HangFire".job(id) ON UPDATE CASCADE ON DELETE CASCADE;
