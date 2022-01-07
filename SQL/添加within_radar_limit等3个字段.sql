alter table t_base_radar_info add column within_radar_limit INTEGER not null on conflict FAIL default 1;
alter table t_base_radar_info add column within_claimer_limit INTEGER not null on conflict FAIL default 1;
alter table t_base_radar_info add column within_angle_limit INTEGER not null on conflict FAIL default 1;