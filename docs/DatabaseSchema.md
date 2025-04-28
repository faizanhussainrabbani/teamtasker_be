# TeamTasker Database Schema

This document provides a detailed overview of the TeamTasker database schema, including all tables, columns, relationships, and constraints.

## Tables Overview

The TeamTasker database consists of the following tables:

1. **Users** - Stores user account information
2. **Projects** - Stores project information
3. **Tasks** - Stores task information
4. **Teams** - Stores team information
5. **TeamMembers** - Stores team membership information
6. **Skills** - Stores skill information
7. **UserSkills** - Junction table for user-skill relationships
8. **TaskTags** - Junction table for task-tag relationships
9. **__EFMigrationsHistory** - Entity Framework migrations tracking table

## Entity Relationship Diagram

```
┌───────────────┐       ┌───────────────┐       ┌───────────────┐
│               │       │               │       │               │
│     Users     │◀──────┤  TeamMembers  ├───────▶     Teams     │
│               │       │               │       │               │
└───────┬───────┘       └───────┬───────┘       └───────────────┘
        │                       │
        │                       │
        │                       │
┌───────▼───────┐       ┌───────▼───────┐
│               │       │               │
│   UserSkills  │       │     Tasks     │
│               │       │               │
└───────┬───────┘       └───────┬───────┘
        │                       │
        │                       │
        ▼                       ▼
┌───────────────┐       ┌───────────────┐       ┌───────────────┐
│               │       │               │       │               │
│    Skills     │       │   TaskTags    │◀──────┤   Projects    │
│               │       │               │       │               │
└───────────────┘       └───────────────┘       └───────────────┘
```

## Detailed Table Schemas

### Users Table

Stores user account information.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| Id | INTEGER | PRIMARY KEY, AUTOINCREMENT | Unique identifier for the user |
| Username | TEXT | NOT NULL | User's login name |
| Email | TEXT | NOT NULL | User's email address |
| PasswordHash | TEXT | NOT NULL | Hashed password |
| FirstName | TEXT | NOT NULL | User's first name |
| LastName | TEXT | NOT NULL | User's last name |
| Avatar | TEXT | NULL | URL to user's avatar image |
| Initials | TEXT | NULL | User's initials |
| PhoneNumber | TEXT | NULL | User's phone number |
| AddressLine1 | TEXT | NULL | First line of user's address |
| AddressLine2 | TEXT | NULL | Second line of user's address |
| City | TEXT | NULL | User's city |
| State | TEXT | NULL | User's state/province |
| PostalCode | TEXT | NULL | User's postal/zip code |
| Country | TEXT | NULL | User's country |
| Role | INTEGER | NOT NULL | User's role (Admin, Manager, Member) |
| Status | INTEGER | NOT NULL | User's status (Active, Inactive, Pending) |
| CreatedDate | TEXT | NOT NULL | Date when the user was created |
| UpdatedDate | TEXT | NOT NULL | Date when the user was last updated |
| LastLoginDate | TEXT | NULL | Date of the user's last login |
| RefreshToken | TEXT | NULL | Token used for authentication refresh |
| RefreshTokenExpiryTime | TEXT | NULL | Expiration time of the refresh token |

### Projects Table

Stores project information.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| Id | INTEGER | PRIMARY KEY, AUTOINCREMENT | Unique identifier for the project |
| Name | TEXT | NOT NULL | Project name |
| Description | TEXT | NOT NULL | Project description |
| Status | INTEGER | NOT NULL | Project status (Active, Completed, OnHold, Cancelled) |
| StartDate | TEXT | NOT NULL | Project start date |
| EndDate | TEXT | NULL | Project end date |
| TeamId | INTEGER | NULL | Foreign key to Teams table |
| CreatedDate | TEXT | NOT NULL | Date when the project was created |
| UpdatedDate | TEXT | NOT NULL | Date when the project was last updated |

### Tasks Table

Stores task information.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| Id | INTEGER | PRIMARY KEY, AUTOINCREMENT | Unique identifier for the task |
| Title | TEXT | NOT NULL | Task title |
| Description | TEXT | NOT NULL | Task description |
| Status | INTEGER | NOT NULL | Task status (ToDo, InProgress, Done, Blocked, OnHold, Cancelled) |
| Priority | INTEGER | NOT NULL | Task priority (Low, Medium, High, Critical) |
| Progress | INTEGER | NOT NULL | Task progress percentage (0-100) |
| DueDate | TEXT | NOT NULL | Task due date |
| CompletedDate | TEXT | NULL | Date when the task was completed |
| ProjectId | INTEGER | NOT NULL | Foreign key to Projects table |
| AssignedToUserId | INTEGER | NULL | Foreign key to Users table (legacy) |
| AssignedToTeamMemberId | INTEGER | NULL | Foreign key to TeamMembers table |
| CreatorTeamMemberId | INTEGER | NULL | Foreign key to TeamMembers table |
| CreatedDate | TEXT | NOT NULL | Date when the task was created |
| UpdatedDate | TEXT | NOT NULL | Date when the task was last updated |

### Teams Table

Stores team information.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| Id | INTEGER | PRIMARY KEY, AUTOINCREMENT | Unique identifier for the team |
| Name | TEXT | NOT NULL | Team name |
| Description | TEXT | NOT NULL | Team description |
| CreatedDate | TEXT | NOT NULL | Date when the team was created |
| UpdatedDate | TEXT | NOT NULL | Date when the team was last updated |

### TeamMembers Table

Stores team membership information.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| Id | INTEGER | PRIMARY KEY, AUTOINCREMENT | Unique identifier for the team member |
| UserId | INTEGER | NOT NULL | Foreign key to Users table |
| TeamId | INTEGER | NOT NULL | Foreign key to Teams table |
| Role | INTEGER | NOT NULL | Team member role (Owner, Admin, Member) |
| JoinDate | TEXT | NOT NULL | Date when the user joined the team |
| CreatedDate | TEXT | NOT NULL | Date when the team member record was created |
| UpdatedDate | TEXT | NOT NULL | Date when the team member record was last updated |

### Skills Table

Stores skill information.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| Id | INTEGER | PRIMARY KEY, AUTOINCREMENT | Unique identifier for the skill |
| Name | TEXT | NOT NULL | Skill name |
| Description | TEXT | NOT NULL | Skill description |
| Category | TEXT | NULL | Skill category |
| CreatedDate | TEXT | NOT NULL | Date when the skill was created |
| UpdatedDate | TEXT | NOT NULL | Date when the skill was last updated |

### UserSkills Table

Junction table for user-skill relationships.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| UserId | INTEGER | NOT NULL | Foreign key to Users table |
| SkillId | INTEGER | NOT NULL | Foreign key to Skills table |
| ProficiencyLevel | INTEGER | NOT NULL | User's proficiency level in the skill |
| YearsOfExperience | INTEGER | NULL | Years of experience with the skill |
| CreatedDate | TEXT | NOT NULL | Date when the user skill record was created |
| UpdatedDate | TEXT | NOT NULL | Date when the user skill record was last updated |

### TaskTags Table

Junction table for task-tag relationships.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| TaskId | INTEGER | NOT NULL | Foreign key to Tasks table |
| Tag | TEXT | NOT NULL | Tag name |

### __EFMigrationsHistory Table

Entity Framework migrations tracking table.

| Column Name | Data Type | Constraints | Description |
|-------------|-----------|-------------|-------------|
| MigrationId | TEXT | PRIMARY KEY | Migration identifier |
| ProductVersion | TEXT | NOT NULL | Entity Framework version |

## Key Relationships

1. **Users to TeamMembers**: One-to-Many (One user can be a member of multiple teams)
2. **Teams to TeamMembers**: One-to-Many (One team can have multiple members)
3. **Users to UserSkills**: One-to-Many (One user can have multiple skills)
4. **Skills to UserSkills**: One-to-Many (One skill can be possessed by multiple users)
5. **Projects to Tasks**: One-to-Many (One project can have multiple tasks)
6. **TeamMembers to Tasks (Creator)**: One-to-Many (One team member can create multiple tasks)
7. **TeamMembers to Tasks (Assignee)**: One-to-Many (One team member can be assigned to multiple tasks)
8. **Tasks to TaskTags**: One-to-Many (One task can have multiple tags)
9. **Teams to Projects**: One-to-Many (One team can have multiple projects)

## Recent Schema Changes

1. **Removed CreatorId from Tasks**: The `CreatorId` column has been removed from the Tasks table in favor of using only `CreatorTeamMemberId` to simplify the task assignment model.

2. **Added TeamMember Relationships**: Tasks are now associated with team members from a dedicated team_members table rather than directly with users.

## Indexes

The following indexes are defined to optimize query performance:

1. **IX_Projects_TeamId**: Index on TeamId in Projects table
2. **IX_Tasks_AssignedToTeamMemberId**: Index on AssignedToTeamMemberId in Tasks table
3. **IX_Tasks_AssignedToUserId**: Index on AssignedToUserId in Tasks table
4. **IX_Tasks_CreatorTeamMemberId**: Index on CreatorTeamMemberId in Tasks table
5. **IX_Tasks_ProjectId**: Index on ProjectId in Tasks table
6. **IX_TaskTags_TaskId**: Index on TaskId in TaskTags table
7. **IX_TeamMembers_TeamId**: Index on TeamId in TeamMembers table
8. **IX_TeamMembers_UserId**: Index on UserId in TeamMembers table
9. **IX_UserSkills_SkillId**: Index on SkillId in UserSkills table
10. **IX_UserSkills_UserId**: Index on UserId in UserSkills table

## Foreign Key Constraints

The following foreign key constraints ensure data integrity:

1. **FK_Projects_Teams_TeamId**: Projects.TeamId references Teams.Id
2. **FK_Tasks_Projects_ProjectId**: Tasks.ProjectId references Projects.Id (CASCADE DELETE)
3. **FK_Tasks_TeamMembers_AssignedToTeamMemberId**: Tasks.AssignedToTeamMemberId references TeamMembers.Id
4. **FK_Tasks_TeamMembers_CreatorTeamMemberId**: Tasks.CreatorTeamMemberId references TeamMembers.Id
5. **FK_Tasks_Users_AssignedToUserId**: Tasks.AssignedToUserId references Users.Id (SET NULL on delete)
6. **FK_TaskTags_Tasks_TaskId**: TaskTags.TaskId references Tasks.Id (CASCADE DELETE)
7. **FK_TeamMembers_Teams_TeamId**: TeamMembers.TeamId references Teams.Id (CASCADE DELETE)
8. **FK_TeamMembers_Users_UserId**: TeamMembers.UserId references Users.Id (CASCADE DELETE)
9. **FK_UserSkills_Skills_SkillId**: UserSkills.SkillId references Skills.Id (CASCADE DELETE)
10. **FK_UserSkills_Users_UserId**: UserSkills.UserId references Users.Id (CASCADE DELETE)
