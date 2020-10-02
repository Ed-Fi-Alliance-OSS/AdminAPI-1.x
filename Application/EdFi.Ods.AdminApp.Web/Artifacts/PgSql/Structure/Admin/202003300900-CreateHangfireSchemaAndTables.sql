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

--
-- TOC entry 9 (class 2615 OID 232641)
-- Name: adminapp_HangFire; Type: SCHEMA; Schema: -; Owner: saagar14
--

CREATE SCHEMA "adminapp_HangFire";


ALTER SCHEMA "adminapp_HangFire" OWNER TO saagar14;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 241 (class 1259 OID 232649)
-- Name: counter; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".counter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp without time zone
);


ALTER TABLE "adminapp_HangFire".counter OWNER TO saagar14;

--
-- TOC entry 240 (class 1259 OID 232647)
-- Name: counter_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".counter_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".counter_id_seq OWNER TO saagar14;

--
-- TOC entry 3045 (class 0 OID 0)
-- Dependencies: 240
-- Name: counter_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".counter_id_seq OWNED BY "adminapp_HangFire".counter.id;


--
-- TOC entry 243 (class 1259 OID 232658)
-- Name: hash; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".hash (
    id bigint NOT NULL,
    key text NOT NULL,
    field text NOT NULL,
    value text,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".hash OWNER TO saagar14;

--
-- TOC entry 242 (class 1259 OID 232656)
-- Name: hash_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".hash_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".hash_id_seq OWNER TO saagar14;

--
-- TOC entry 3046 (class 0 OID 0)
-- Dependencies: 242
-- Name: hash_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".hash_id_seq OWNED BY "adminapp_HangFire".hash.id;


--
-- TOC entry 245 (class 1259 OID 232671)
-- Name: job; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".job (
    id bigint NOT NULL,
    stateid bigint,
    statename text,
    invocationdata text NOT NULL,
    arguments text NOT NULL,
    createdat timestamp without time zone NOT NULL,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".job OWNER TO saagar14;

--
-- TOC entry 244 (class 1259 OID 232669)
-- Name: job_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".job_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".job_id_seq OWNER TO saagar14;

--
-- TOC entry 3047 (class 0 OID 0)
-- Dependencies: 244
-- Name: job_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".job_id_seq OWNED BY "adminapp_HangFire".job.id;


--
-- TOC entry 256 (class 1259 OID 232741)
-- Name: jobparameter; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".jobparameter (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    value text,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".jobparameter OWNER TO saagar14;

--
-- TOC entry 255 (class 1259 OID 232739)
-- Name: jobparameter_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".jobparameter_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".jobparameter_id_seq OWNER TO saagar14;

--
-- TOC entry 3048 (class 0 OID 0)
-- Dependencies: 255
-- Name: jobparameter_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".jobparameter_id_seq OWNED BY "adminapp_HangFire".jobparameter.id;


--
-- TOC entry 249 (class 1259 OID 232700)
-- Name: jobqueue; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".jobqueue (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    queue text NOT NULL,
    fetchedat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".jobqueue OWNER TO saagar14;

--
-- TOC entry 248 (class 1259 OID 232698)
-- Name: jobqueue_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".jobqueue_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".jobqueue_id_seq OWNER TO saagar14;

--
-- TOC entry 3049 (class 0 OID 0)
-- Dependencies: 248
-- Name: jobqueue_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".jobqueue_id_seq OWNED BY "adminapp_HangFire".jobqueue.id;


--
-- TOC entry 251 (class 1259 OID 232709)
-- Name: list; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".list (
    id bigint NOT NULL,
    key text NOT NULL,
    value text,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".list OWNER TO saagar14;

--
-- TOC entry 250 (class 1259 OID 232707)
-- Name: list_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".list_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".list_id_seq OWNER TO saagar14;

--
-- TOC entry 3050 (class 0 OID 0)
-- Dependencies: 250
-- Name: list_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".list_id_seq OWNED BY "adminapp_HangFire".list.id;


--
-- TOC entry 257 (class 1259 OID 232756)
-- Name: lock; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".lock (
    resource text NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL,
    acquired timestamp without time zone
);


ALTER TABLE "adminapp_HangFire".lock OWNER TO saagar14;

--
-- TOC entry 239 (class 1259 OID 232642)
-- Name: schema; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".schema (
    version integer NOT NULL
);


ALTER TABLE "adminapp_HangFire".schema OWNER TO saagar14;

--
-- TOC entry 252 (class 1259 OID 232718)
-- Name: server; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".server (
    id text NOT NULL,
    data text,
    lastheartbeat timestamp without time zone NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".server OWNER TO saagar14;

--
-- TOC entry 254 (class 1259 OID 232728)
-- Name: set; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".set (
    id bigint NOT NULL,
    key text NOT NULL,
    score double precision NOT NULL,
    value text NOT NULL,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".set OWNER TO saagar14;

--
-- TOC entry 253 (class 1259 OID 232726)
-- Name: set_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".set_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".set_id_seq OWNER TO saagar14;

--
-- TOC entry 3051 (class 0 OID 0)
-- Dependencies: 253
-- Name: set_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".set_id_seq OWNED BY "adminapp_HangFire".set.id;


--
-- TOC entry 247 (class 1259 OID 232683)
-- Name: state; Type: TABLE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE TABLE "adminapp_HangFire".state (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    reason text,
    createdat timestamp without time zone NOT NULL,
    data text,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE "adminapp_HangFire".state OWNER TO saagar14;

--
-- TOC entry 246 (class 1259 OID 232681)
-- Name: state_id_seq; Type: SEQUENCE; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE SEQUENCE "adminapp_HangFire".state_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "adminapp_HangFire".state_id_seq OWNER TO saagar14;

--
-- TOC entry 3052 (class 0 OID 0)
-- Dependencies: 246
-- Name: state_id_seq; Type: SEQUENCE OWNED BY; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER SEQUENCE "adminapp_HangFire".state_id_seq OWNED BY "adminapp_HangFire".state.id;


--
-- TOC entry 2867 (class 2604 OID 232791)
-- Name: counter id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".counter ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".counter_id_seq'::regclass);


--
-- TOC entry 2869 (class 2604 OID 232800)
-- Name: hash id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".hash ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".hash_id_seq'::regclass);


--
-- TOC entry 2871 (class 2604 OID 232811)
-- Name: job id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".job ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".job_id_seq'::regclass);


--
-- TOC entry 2882 (class 2604 OID 232865)
-- Name: jobparameter id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".jobparameter ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".jobparameter_id_seq'::regclass);


--
-- TOC entry 2875 (class 2604 OID 232890)
-- Name: jobqueue id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".jobqueue ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".jobqueue_id_seq'::regclass);


--
-- TOC entry 2877 (class 2604 OID 232912)
-- Name: list id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".list ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".list_id_seq'::regclass);


--
-- TOC entry 2880 (class 2604 OID 232922)
-- Name: set id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".set ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".set_id_seq'::regclass);


--
-- TOC entry 2873 (class 2604 OID 232840)
-- Name: state id; Type: DEFAULT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".state ALTER COLUMN id SET DEFAULT nextval('"adminapp_HangFire".state_id_seq'::regclass);


--
-- TOC entry 2887 (class 2606 OID 232793)
-- Name: counter counter_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);


--
-- TOC entry 2891 (class 2606 OID 232940)
-- Name: hash hash_key_field_key; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);


--
-- TOC entry 2893 (class 2606 OID 232802)
-- Name: hash hash_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);


--
-- TOC entry 2896 (class 2606 OID 232813)
-- Name: job job_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- TOC entry 2914 (class 2606 OID 232867)
-- Name: jobparameter jobparameter_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);


--
-- TOC entry 2903 (class 2606 OID 232892)
-- Name: jobqueue jobqueue_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);


--
-- TOC entry 2905 (class 2606 OID 232914)
-- Name: list list_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);


--
-- TOC entry 2916 (class 2606 OID 232782)
-- Name: lock lock_resource_key; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);


--
-- TOC entry 2885 (class 2606 OID 232646)
-- Name: schema schema_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);


--
-- TOC entry 2907 (class 2606 OID 232943)
-- Name: server server_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);


--
-- TOC entry 2909 (class 2606 OID 232945)
-- Name: set set_key_value_key; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);


--
-- TOC entry 2911 (class 2606 OID 232924)
-- Name: set set_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);


--
-- TOC entry 2899 (class 2606 OID 232842)
-- Name: state state_pkey; Type: CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);


--
-- TOC entry 2888 (class 1259 OID 232773)
-- Name: ix_hangfire_counter_expireat; Type: INDEX; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE INDEX ix_hangfire_counter_expireat ON "adminapp_HangFire".counter USING btree (expireat);


--
-- TOC entry 2889 (class 1259 OID 232933)
-- Name: ix_hangfire_counter_key; Type: INDEX; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE INDEX ix_hangfire_counter_key ON "adminapp_HangFire".counter USING btree (key);


--
-- TOC entry 2894 (class 1259 OID 232941)
-- Name: ix_hangfire_job_statename; Type: INDEX; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE INDEX ix_hangfire_job_statename ON "adminapp_HangFire".job USING btree (statename);


--
-- TOC entry 2912 (class 1259 OID 232946)
-- Name: ix_hangfire_jobparameter_jobidandname; Type: INDEX; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON "adminapp_HangFire".jobparameter USING btree (jobid, name);


--
-- TOC entry 2900 (class 1259 OID 232902)
-- Name: ix_hangfire_jobqueue_jobidandqueue; Type: INDEX; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON "adminapp_HangFire".jobqueue USING btree (jobid, queue);


--
-- TOC entry 2901 (class 1259 OID 232787)
-- Name: ix_hangfire_jobqueue_queueandfetchedat; Type: INDEX; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON "adminapp_HangFire".jobqueue USING btree (queue, fetchedat);


--
-- TOC entry 2897 (class 1259 OID 232851)
-- Name: ix_hangfire_state_jobid; Type: INDEX; Schema: adminapp_HangFire; Owner: saagar14
--

CREATE INDEX ix_hangfire_state_jobid ON "adminapp_HangFire".state USING btree (jobid);


--
-- TOC entry 2918 (class 2606 OID 232877)
-- Name: jobparameter jobparameter_jobid_fkey; Type: FK CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES "adminapp_HangFire".job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2917 (class 2606 OID 232852)
-- Name: state state_jobid_fkey; Type: FK CONSTRAINT; Schema: adminapp_HangFire; Owner: saagar14
--

ALTER TABLE ONLY "adminapp_HangFire".state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES "adminapp_HangFire".job(id) ON UPDATE CASCADE ON DELETE CASCADE;
