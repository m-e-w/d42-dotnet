/*
For RU data and metrics.
*/
select 
    r.last_updated "RU Data Last Updated",
    d.name "Device Name",
    d.monitoring_enabled "Monitoring Enabled",
    r.rudata_pk "Resource ID",
    r.value "Resource Value",
    r.sensor_type "Resource Type",
    r.sensor "Resource",
    r.measure_type "Measurement Type",
    r.metric "Measurement Metric",
    r.timeperiod_id "Time Period ID",
    r.timeperiod "Time Period",
    rc.name "Remote Collector Name",
    rc.ip "Remote Collector IP"
    from view_device_v1 d
    left join view_rudata_v1 r on r.device_fk = d.device_pk
    left join view_remotecollector_v1 rc on rc.remotecollector_pk = r.remotecollector_fk
    order by d.monitoring_enabled DESC, d.name ASC, r.timeperiod_id ASC, r.measure_type ASC, r.metric ASC