CREATE TABLE [t_plc_opcgroup](
  [group_id] INTEGER PRIMARY KEY ON CONFLICT FAIL AUTOINCREMENT NOT NULL ON CONFLICT FAIL, 
  [group_name] VARCHAR2(32) NOT NULL ON CONFLICT FAIL, 
  [group_type] INTEGER(1) NOT NULL ON CONFLICT FAIL DEFAULT 1);
CREATE TABLE [t_plc_opcitem](
  [record_id] INTEGER PRIMARY KEY ON CONFLICT FAIL AUTOINCREMENT NOT NULL ON CONFLICT FAIL, 
  [item_id] VARCHAR2(64) NOT NULL ON CONFLICT FAIL, 
  [opcgroup_id] INTEGER NOT NULL ON CONFLICT FAIL, 
  [field_name] VARCHAR2(64), 
  [enabled] INTEGER NOT NULL ON CONFLICT FAIL DEFAULT 1, 
  [coeff] NUMBER NOT NULL ON CONFLICT FAIL DEFAULT 0);
--OPC组
INSERT INTO t_plc_opcgroup (group_id, group_name, group_type) VALUES (1, 'OPC_GROUP_READ', 1);
INSERT INTO t_plc_opcgroup (group_id, group_name, group_type) VALUES (2, 'OPC_GROUP_WRITE', 2);
--OPC项
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]ENCODER_DATA[2]', 1, 'WalkingPosition_Plc', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]ENCODER_DATA[0]', 1, 'PitchAngle_Plc', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]ENCODER_DATA[1]', 1, 'YawAngle_Plc', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]Local:4:I.Data[18]', 1, 'StretchLength_Plc', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]WRH_Radar_Pile_Height', 1, 'PileHeight_Plc', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.2', 1, 'WalkBackward', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.1', 1, 'WalkFixated', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.0', 1, 'WalkForward', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.15', 1, 'PitchDownward', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.14', 1, 'PitchFixated', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.13', 1, 'PitchUpward', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.10', 1, 'YawLeft', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.11', 1, 'YawFixated', 1);
INSERT INTO t_plc_opcitem (item_id, opcgroup_id, field_name, enabled) VALUES ('[R1_TOPIC]rack11:1:I.Data.12', 1, 'YawRight', 1);
--更新TOPIC
update t_plc_opcitem set item_id = replace(item_id, 'R1_TOPIC', 'R1_TOPIC');