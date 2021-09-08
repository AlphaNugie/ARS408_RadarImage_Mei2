update t_base_radar_info set rcs_min = -25, rcs_max = 64, refresh_interval = 50, apply_filter = 1, apply_iteration = 1, claimer_coors_limited = 1,
                             claimer_y_min = -50, claimer_y_max = 50, claimer_z_min = -5, claimer_z_max = 5, pushf_max_count = 10, use_public_filters = 0,
                             false_alarm_filter = '0,1,2,3', ambig_state_filter = '0,1,2,3,4', invalid_state_filter = '0,1,2,3,4,5,6,8,9,10,11,12,13,14,15,16,17'
                         where owner_group_id = 9 and direction_id != 6;
update t_base_radar_info set claimer_x_min = -50, claimer_x_max = -4 where owner_group_id = 9 and direction_id = 2;
update t_base_radar_info set claimer_x_min = 4, claimer_x_max = 50 where owner_group_id = 9 and direction_id = 4;