/*
2019-10-25 Servers Activity and Utilization
Returns discovery, RU, and service connection information to give a sense of when this device
was last discovered or how it has been used. Includes fields like last attempted/successful discovery,
last service comms and software install, last login, general device details and RU data. 
Useful in identifying underutilized or zombie servers.
*/


select
	ds.updated "Last Attempted Discovery",
	(select max(scl.last_detected) from view_servicecommunication_v2 scl where d.device_pk = scl.listener_device_fk) "Last Detected Comms (Listener)",
	(select max(scc.last_detected) from view_servicecommunication_v2 scc where d.device_pk = scc.client_device_fk) "Last Detected Comms (Client)",
	(select max(siu.install_date) from view_softwareinuse_v1 siu where d.device_pk = siu.device_fk) "Last Software Installation",
	d.*,
	l.last_login,
	l.domain,
	l.username
from
    (
        select
            d.device_pk,
            d.last_edited "Last Successful Discovery",
            d.name "Device Name",
            d.virtual_subtype "Virtual Subtype",
            d.os_name "OS Name",
            d.os_version_no "OS Version",
            d.os_arch "OS Architecture",
            d.cpucount "CPU Count",
            d.cpucore "Cores Per Socket",
            CASE
               WHEN d.in_service = 't'
               THEN
               'YES'
               ELSE
               'NO'
            END "In Service?",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '1' and ru.metric_id = '3'))::numeric, 2) "1 Day CPU AVG (%)",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '1' and ru.metric_id = '1'))::numeric, 2) "1 Day CPU MAX (%)",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '2' and ru.metric_id = '3'))::numeric, 2) "7 Day CPU AVG (%)",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '2' and ru.metric_id = '1'))::numeric, 2) "7 Day CPU MAX (%)",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '3' and ru.metric_id = '3'))::numeric, 2) "30 Day CPU AVG (%)",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '3' and ru.metric_id = '1'))::numeric, 2) "30 Day CPU MAX (%)",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '4' and ru.metric_id = '3'))::numeric, 2) "90 Day CPU AVG (%)",
            round(sum((select ru.value where ru.measure_type_id = '1' and ru.timeperiod_id = '4' and ru.metric_id = '1'))::numeric, 2) "90 Day CPU MAX (%)", 
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '1' and ru.metric_id = '3'))::numeric, 2) "1 Day MEM AVG (%)",
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '1' and ru.metric_id = '1'))::numeric, 2) "1 Day MEM MAX (%)",
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '2' and ru.metric_id = '3'))::numeric, 2) "7 Day MEM AVG (%)",
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '2' and ru.metric_id = '1'))::numeric, 2) "7 Day MEM MAX (%)",
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '3' and ru.metric_id = '3'))::numeric, 2) "30 Day MEM AVG (%)",
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '3' and ru.metric_id = '1'))::numeric, 2) "30 Day MEM MAX (%)",
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '4' and ru.metric_id = '3'))::numeric, 2) "90 Day MEM AVG (%)",
            round(sum((select ru.value * 100 / (d.ram * 1024) where ru.measure_type_id = '2' and ru.timeperiod_id = '4' and ru.metric_id = '1'))::numeric, 2) "90 Day MEM MAX (%)",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '1' and ru.metric_id = '3'))::numeric, 2) "1 Day DISK IO Read AVG",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '1' and ru.metric_id = '1'))::numeric, 2) "1 Day DISK IO Read MAX",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '2' and ru.metric_id = '3'))::numeric, 2) "7 Day DISK IO Read AVG",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '2' and ru.metric_id = '1'))::numeric, 2) "7 Day DISK IO Read MAX",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '3' and ru.metric_id = '3'))::numeric, 2) "30 Day DISK IO Read AVG",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '3' and ru.metric_id = '1'))::numeric, 2) "30 Day DISK IO Read MAX",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '4' and ru.metric_id = '3'))::numeric, 2) "90 Day DISK IO Read AVG",
            round(sum((select ru.value where ru.measure_type_id = '3' and ru.timeperiod_id = '4' and ru.metric_id = '1'))::numeric, 2) "90 Day DISK IO Read MAX",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '1' and ru.metric_id = '3'))::numeric, 2) "1 Day DISK IO Write AVG",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '1' and ru.metric_id = '1'))::numeric, 2) "1 Day DISK IO Write MAX",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '2' and ru.metric_id = '3'))::numeric, 2) "7 Day DISK IO Write AVG",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '2' and ru.metric_id = '1'))::numeric, 2) "7 Day DISK IO Write MAX",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '3' and ru.metric_id = '3'))::numeric, 2) "30 Day DISK IO Write AVG",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '3' and ru.metric_id = '1'))::numeric, 2) "30 Day DISK IO Write MAX",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '4' and ru.metric_id = '3'))::numeric, 2) "90 Day DISK IO Write AVG",
            round(sum((select ru.value where ru.measure_type_id = '4' and ru.timeperiod_id = '4' and ru.metric_id = '1'))::numeric, 2) "90 Day DISK IO Write MAX",
            round(sum((select ru.value where ru.measure_type_id = '9' and ru.timeperiod_id = '1' and ru.metric_id = '4'))::numeric, 2) "1 Day NIC Transfer IN",
            round(sum((select ru.value where ru.measure_type_id = '10' and ru.timeperiod_id = '1' and ru.metric_id = '4'))::numeric, 2) "1 Day NIC Transfer OUT",
            round(sum((select ru.value where ru.measure_type_id = '9' and ru.timeperiod_id = '2' and ru.metric_id = '4'))::numeric, 2) "7 Day NIC Transfer IN",
            round(sum((select ru.value where ru.measure_type_id = '10' and ru.timeperiod_id = '2' and ru.metric_id = '4'))::numeric, 2) "7 Day NIC Transfer OUT",
            round(sum((select ru.value where ru.measure_type_id = '9' and ru.timeperiod_id = '3' and ru.metric_id = '4'))::numeric, 2) "30 Day NIC Transfer IN",
            round(sum((select ru.value where ru.measure_type_id = '10' and ru.timeperiod_id = '3' and ru.metric_id = '4'))::numeric, 2) "30 Day NIC Transfer OUT",
            round(sum((select ru.value where ru.measure_type_id = '9' and ru.timeperiod_id = '4' and ru.metric_id = '4'))::numeric, 2) "90 Day NIC Transfer IN",
            round(sum((select ru.value where ru.measure_type_id = '10' and ru.timeperiod_id = '4' and ru.metric_id = '4'))::numeric, 2) "90 Day NIC Transfer OUT"
        from 
            view_device_v1 d
            left join view_rudata_v1 ru on d.device_pk = ru.device_fk
        group by
            d.device_pk,
            "Last Successful Discovery",
            "Device Name",
            "Virtual Subtype",
            "OS Name",
            "OS Version",
            "OS Architecture",
            "CPU Count",
            "Cores Per Socket",
            "In Service?"
        Order by d.name ASC
    ) d
    join view_discoveryscores_v1 ds on ds.device_fk = d.device_pk and
									   ds.added = (select max(lds.added) from view_discoveryscores_v1 lds where lds.device_fk = d.device_pk)	
    join view_jobscore_v1 js on ds.jobscore_fk = js.jobscore_pk and
							    js.jobscore_pk = (select max(ljs.jobscore_pk) from view_jobscore_v1 ljs where ljs.jobscore_pk = ds.jobscore_fk)
    join view_devicelastlogin_v1 l on l.device_fk = d.device_pk and 
                                      l.last_login = (select max(lr.last_login) from view_devicelastlogin_v1 lr where lr.device_fk = d.device_pk)   
where 
    js.jobscore_pk in (select max(jobscore_pk) from view_jobscore_v1 jk group by jk.vserverdiscovery_fk)