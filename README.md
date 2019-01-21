# PHPSwitch

Switch between PHP versions using command line on Windows.

## Requirement:
Windows (I'm not sure that 7, 8, or 10 at least) 64 bit.<br>
.NET Framework 4.6.1<br>
Administrator privilege (Windows UAC).

## Installation

### Prepare folders
#### PHP versions folder
There is one folder that contain multiple PHP versions and this is required. This folder will be called **PHP versions folder**.

The **PHP versions folder** structor must follow this guide line.

* php *(Assume that this is PHP versions folder)*.
	* php5.5 *(No need to be same version but keep the version number after the text "php" in lower case, no space)*
		* dev
		* ext
		* *...*
		* php.ini
		* *... & more.*
     * php5.6
     	* *...*
     * php7.0
     * php7.1
     * *... & more.*
	 * php-running

Each PHP versions folder must contain the files that you have downloaded from [php.net](https://php.net).

The **php-running** folder is required to make PHP works (CLI and web server).<br>
If you want to access `php` executable file, please add path to this folder into your system variable **Path**.<br>
To do this, run `rundll32.exe sysdm.cpl,EditEnvironmentVariables` in the command line. Edit **Path** in **System variables** and then add the path to **php-running** folder.

#### Apache folder
Another one is the folder that contain Apache but this is optional for who is using Apache only. This folder will be called **Apache folder**.

The **Apache folder** structor must follow this guide line.

* Apache24 *24 represent Apache v2.4*
	* bin
	* cgi-bin
	* conf
		* extra
			* *...*
			* httpd-php.conf *This file is required if you use Apache.*
			* httpd-php-5.5.conf *(No need to be same version but keep the version number after the text "php" in lower case, no space)*
			* httpd-php-5.6.conf
			* httpd-php-7.0.conf
			* httpd-php-7.1.conf
			* *... & more.*
		* original
		* *... & more.*
	* *... & more.*

The file **httpd-php.conf** must be included in the **httpd.conf** file. The file **httpd-php-x.x.conf** (where x.x is the version of PHP) must be included in the **httpd-php.conf** file, but this will be automatically generate by **PHPSwitch** application.

#### PHP+Apache (optional)+PHPSwitch
Put all these folders together for easily to use the command.

* apache
	* Apache24
		* bin
		* cgi-bin
		* conf
			* extra
			* *... & more.*
        * *... & more.*
* php
	* php5.5
	* php5.6
	* php7.0
	* *... & more.*
* phpswitch
	* phpswitch.exe

## Usage:
Run `phpswitch` command and follow with version number. Example: `phpswitch 7.1` to switch to PHP 7.1.

![PHPSwitch Screenshot](.web-assets/phpswitch0.1-screenshot-01.jpg)

If you have **PHP versions folder** or **Apache folder** on different location, please use the following command.

```
phpswitch 7.1 C:\myphp-versions
```
The above command will be use any phpx.x folders inside **C:\myphp-versions**

```
phpswitch 7.2 C:\myphp-versions C:\myapache\Apache2.4
```
The above command has additional path to Apache folder. This folder must contain conf folder in it.