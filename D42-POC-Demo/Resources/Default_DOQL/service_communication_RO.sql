/*
Query for all discovered connections to service instances.
*/
select
ld.name "Listening Device",
ag.name "Affinity Group",
sc.listener_ip "Listening IP",
lp.port "Listening Port",
s.displayname "Listening Service",
sc.port "Port Communication",
sc.protocol "Protocol",
cd.name "Client Device",
sc.client_ip "Client IP",
sc.port "Client Port Communication",
sc.client_process_display_name "Process Display Name",
sc.client_process_name "Process Name",
sc.last_detected "Communication Last Detected",
sc.netstat_active_samples "Netstat # Times Port Actively Connected at Discovery",
sc.netstat_total_samples "Netstat # Times Discovery Checked for Connection",
sc.netstat_total_eports "Netstat Total # Port Connections at Discovery",
sc.netstat_all_first_stat "Netstat Time First Time Port Connected at Discovery",
sc.netstat_all_last_stat "Netstat Last Time Port Connected at Discovery"
from view_servicecommunication_v2 sc
join view_device_v1 ld on ld.device_pk = sc.listener_device_fk
join view_servicelistenerport_v2 lp on lp.servicelistenerport_pk = sc.servicelistenerport_fk
join view_serviceinstance_v2 si on si.serviceinstance_pk = lp.discovered_serviceinstance_fk
join view_service_v2 s on s.service_pk = si.service_fk
left join view_device_v1 cd on cd.device_pk = sc.client_device_fk
left join view_affinitygroup_v2 ag on ag.primary_device_fk = ld.device_pk
where sc.client_ip != '127.0.0.1' and sc.client_ip != '::1'